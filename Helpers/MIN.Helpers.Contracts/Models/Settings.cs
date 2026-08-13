using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MIN.Helpers.Contracts.Models;

/// <summary>
/// Настройки
/// </summary>
public class Settings : INotifyPropertyChanged
{
    private int inputDeviceVolume = 100;
    private int inputDeviceSensitivity = -40;
    private int outputDeviceVolume = 100;

    /// <summary>
    /// Поле приобрело новое значение
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Имя своего участника по умолчанию
    /// </summary>
    public string DefaultParticipantName { get; set; } = string.Empty;

    /// <summary>
    /// Включена ли светлая тема
    /// </summary>
    public bool LightThemeEnabled { get; set; }

    /// <summary>
    /// Время ожидания поиска комнаты
    /// </summary>
    public int DiscoveryTimeout { get; set; } = 1500;

    /// <summary>
    /// Порт для обнаружения в сети
    /// </summary>
    public int DiscoveryPort { get; set; } = 42069;

    /// <summary>
    /// Индекс выбранного микрофона
    /// </summary>
    public int InputDeviceNumber { get; set; }

    /// <summary>
    /// Индекс выбранного динамика
    /// </summary>
    public int OutputDeviceNumber { get; set; }

    /// <summary>
    /// Громкость микрофона
    /// </summary>
    public int InputDeviceVolume
    {
        get => inputDeviceVolume;
        set
        {
            if (inputDeviceVolume != value)
            {
                inputDeviceVolume = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Громкость динамиков всего приложения
    /// </summary>
    public int OutputDeviceVolume
    {
        get => outputDeviceVolume;
        set
        {
            if (outputDeviceVolume != value)
            {
                outputDeviceVolume = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Чувствительность ввода микрофона (дБ)
    /// Отрицательное число в диапозоне от (-60 до 0)
    /// </summary>
    public int InputDeviceSensitivity
    {
        get => inputDeviceSensitivity;
        set
        {
            if (inputDeviceSensitivity != value)
            {
                inputDeviceSensitivity = value;
                OnPropertyChanged();
            }
        }
    }

    /// <inheritdoc />
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
