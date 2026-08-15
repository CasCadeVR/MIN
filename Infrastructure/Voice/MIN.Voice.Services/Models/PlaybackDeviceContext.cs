using OpenTK.Audio.OpenAL;

namespace MIN.Voice.Services.Models;

/// <summary>
/// Владеет ЕДИНСТВЕННЫМ устройством воспроизведения и ЕДИНСТВЕННЫМ AL-контекстом
/// для всего приложения. Все <see cref="ParticipantChannel"/> используют общие
/// device/context и получают только свой собственный source + буферы.
///
/// Это устраняет гонку "текущего контекста" в OpenAL: alcMakeContextCurrent
/// задаёт ГЛОБАЛЬНОЕ (на процесс) текущее состояние, а не per-object. Если у
/// каждого участника свой контекст, конкурентные вызовы AL.* от разных участников
/// молча бьют не по тому контексту.
/// </summary>
public sealed class PlaybackDeviceContext : IDisposable
{
    // Все обращения к ALC/AL для смены устройства и обеспечения "текущести"
    // контекста идут через этот lock - это единственное место в приложении,
    // которое трогает alcMakeContextCurrent.
    private readonly object gate = new();

    private ALDevice device;
    private ALContext context;
    private string? currentDeviceName;

    /// <summary>
    /// Срабатывает после успешной смены устройства/контекста. Все существующие
    /// <see cref="ParticipantChannel"/> должны пересоздать свои source+буферы.
    /// </summary>
    public event Action? DeviceChanged;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="PlaybackDeviceContext"/>
    /// </summary>
    public PlaybackDeviceContext(string? deviceName = null)
    {
        lock (gate)
        {
            Open(deviceName);
        }
    }

    private void Open(string? deviceName)
    {
        var newDevice = ALC.OpenDevice(deviceName);
        if (newDevice == ALDevice.Null)
        {
            throw new InvalidOperationException($"Не удалось открыть устройство воспроизведения: '{deviceName ?? "по умолчанию"}'");
        }

        var newContext = ALC.CreateContext(newDevice, (int[]?)null);
        if (newContext == ALContext.Null)
        {
            ALC.CloseDevice(newDevice);
            throw new InvalidOperationException("Не удалось создать аудио контекст");
        }

        if (!ALC.MakeContextCurrent(newContext))
        {
            ALC.DestroyContext(newContext);
            ALC.CloseDevice(newDevice);
            throw new InvalidOperationException("Не удалось сделать контекст текущим");
        }

        device = newDevice;
        context = newContext;
        currentDeviceName = deviceName;
    }

    /// <summary>
    /// Гарантирует, что общий контекст является текущим для вызывающего потока
    /// перед выполнением AL-вызовов. Дешёвая идемпотентная операция при
    /// единственном контексте в приложении - но обязательна, т.к. "текущий
    /// контекст" глобален и его может (в теории) выставить сторонний код.
    /// Вызывать держа lock не нужно - берётся
    /// собственный gate.
    /// </summary>
    public void EnsureCurrent()
    {
        lock (gate)
        {
            ALC.MakeContextCurrent(context);
        }
    }

    /// <summary>
    /// Выполняет действие под общим gate, гарантируя текущий контекст.
    /// Используйте для любой последовательности AL-вызовов одного участника,
    /// чтобы никто другой не переключил контекст между вашими вызовами.
    /// </summary>
    public void RunExclusive(Action action)
    {
        lock (gate)
        {
            ALC.MakeContextCurrent(context);
            action();
        }
    }
    /// <summary>
    /// Выполняет действие под общим gate, гарантируя текущий контекст.
    /// Используйте для любой последовательности AL-вызовов одного участника,
    /// чтобы никто другой не переключил контекст между вашими вызовами.
    /// </summary>
    public T RunExclusive<T>(Func<T> func)
    {
        lock (gate)
        {
            ALC.MakeContextCurrent(context);
            return func();
        }
    }

    /// <summary>
    /// Меняет устройство воспроизведения для ВСЕГО приложения. Все участники
    /// получат <see cref="DeviceChanged"/> и должны пересоздать свои source.
    /// </summary>
    public void ChangeDevice(string? newDeviceName)
    {
        lock (gate)
        {
            if (currentDeviceName == newDeviceName)
            {
                return;
            }

            var oldDevice = device;
            var oldContext = context;

            Open(newDeviceName); // обновляет device/context/currentDeviceName на новые

            ALC.MakeContextCurrent(ALContext.Null);
            ALC.DestroyContext(oldContext);
            ALC.CloseDevice(oldDevice);

            ALC.MakeContextCurrent(context);
        }

        // Вне lock, чтобы подписчики (ParticipantChannel.RecreateSource) могли
        // сами безопасно взять gate через RunExclusive/EnsureCurrent.
        DeviceChanged?.Invoke();
    }

    /// <inheritdoc cref="IDisposable.Dispose"/>
    public void Dispose()
    {
        lock (gate)
        {
            if (context == ALContext.Null)
            {
                return;
            }

            ALC.MakeContextCurrent(ALContext.Null);
            ALC.DestroyContext(context);
            ALC.CloseDevice(device);
            context = ALContext.Null;
            device = ALDevice.Null;
        }
    }
}
