using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Styling;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MIN.Desktop.Contracts.Constants;
using MIN.Desktop.Contracts.Enums;
using MIN.Desktop.Contracts.Interfaces;
using MIN.Desktop.Infrastructure.Validators;
using MIN.Desktop.ViewModels.Base;
using MIN.Desktop.ViewModels.Modals;
using MIN.DI.FeatureCollection;
using MIN.Helpers.Contracts.Models;

namespace MIN.Desktop.ViewModels.Pages;

/// <summary>
/// Модель боковой панели настроек
/// </summary>
public partial class SettingsSideBarViewModel : ValidatingRoutableViewModelBase
{
    private readonly IMinFeatureCollection featureCollection;
    private readonly IDialogService dialogService;
    private readonly CancellationTokenSource appCts = null!;

    /// <inheritdoc />
    public override ViewLayoutType LayoutType => ViewLayoutType.LeftSideBar;

    /// <summary>
    /// Текущие настройки
    /// </summary>
    [ObservableProperty]
    public partial Settings Settings { get; set; } = null!;

    /// <summary>
    /// Микрофоны
    /// </summary>
    public AvaloniaList<string> InputDevices { get; set; } = [];

    /// <summary>
    /// Динамики
    /// </summary>
    public AvaloniaList<string> OutputDevices { get; set; } = [];

    /// <summary>
    /// Версия приложения
    /// </summary>
    [ObservableProperty]
    public partial string Version { get; set; } = string.Empty;

    /// <summary>
    /// Имя своего участника по умолчанию
    /// </summary>
    [ObservableProperty]
    [Display(Name = "Имя участника")]
    [NotifyDataErrorInfo]
    [ParticipantName]
    [NotEndsWith(".")]
    public partial string DefaultParticipantName { get; set; } = string.Empty;

    /// <summary>
    /// Время ожидания поиска комнаты
    /// </summary>
    [ObservableProperty]
    [IntValue]
    [Range(100, DesktopConstants.RoomConnectionTimeoutMs, ErrorMessage = "Время ожидания поиска комнаты должно быть от 100 до 10000 миллисекунд")]
    [NotifyDataErrorInfo]
    public partial int DiscoveryTimeout { get; set; }

    /// <summary>
    /// Порт для обнаружения в сети
    /// </summary>
    [ObservableProperty]
    [IntValue]
    [Range(1, ushort.MaxValue, ErrorMessage = "Порт должен быть от 1 до 65535")]
    [NotifyDataErrorInfo]
    public partial int DiscoveryPort { get; set; }

    /// <summary>
    /// Включена ли светлая тема
    /// </summary>
    [ObservableProperty]
    public partial bool LightThemeEnabled { get; set; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="SettingsSideBarViewModel"/>
    /// </summary>
    public SettingsSideBarViewModel(IMinFeatureCollection featureCollection,
        IDialogService dialogService,
        ICtsProvider ctsProvider)
    {
        this.featureCollection = featureCollection;
        this.dialogService = dialogService;

        if (!Design.IsDesignMode)
        {
            appCts = ctsProvider.AppCts;

            var inputDeviceNames = featureCollection.Voice.AudioDeviceService.GetInputDevices(asDecoded: true).Select(x => x.Name);
            foreach (var name in inputDeviceNames)
            {
                InputDevices.Add(name);
            }

            var outputDeviceNames = featureCollection.Voice.AudioDeviceService.GetOutputDevices(asDecoded: true).Select(x => x.Name);
            foreach (var name in outputDeviceNames)
            {
                OutputDevices.Add(name);
            }

            featureCollection.Helper.SettingsProvider.OnSettingsSaved += FillControls;

            FillControls();

            Application.Current!.RequestedThemeVariant = Settings.LightThemeEnabled
                ? ThemeVariant.Light
                : ThemeVariant.Dark;
        }
    }

    partial void OnLightThemeEnabledChanged(bool value)
    {
        Dispatcher.UIThread.Invoke(() => Application.Current!.RequestedThemeVariant = value ? ThemeVariant.Light : ThemeVariant.Dark);
    }

    /// <summary>
    /// Вернуться назад
    /// </summary>
    [RelayCommand]
    public void Back()
    {
        if (CanSave())
        {
            Settings.DefaultParticipantName = DefaultParticipantName;
            Settings.DiscoveryPort = DiscoveryPort;
            Settings.DiscoveryTimeout = DiscoveryTimeout;
        }

        Settings.LightThemeEnabled = LightThemeEnabled;

        featureCollection.Helper.SettingsProvider.SaveSettings(Settings);
        ChangeViewToPrevious();
    }

    /// <summary>
    /// Открыть окно логов
    /// </summary>
    [RelayCommand]
    public async Task OpenLogsAsync()
    {
        await dialogService.ShowAsync<LogViewModel>();
        ChangeViewToPrevious();
    }

    /// <summary>
    /// Очистить кэш
    /// </summary>
    [RelayCommand]
    public void ClearCacheAsync()
    {
        featureCollection.Helper.AppDataProvider.ClearFolder("cryptography");
        featureCollection.Helper.AppDataProvider.ClearFolder("network");
    }

    /// <summary>
    /// Отсканировать папку с сессиями
    /// </summary>
    [RelayCommand]
    public async Task ScanSessionsAsync() => await featureCollection.Chat.ChatSessionService.ScanDownloadedSessions(appCts.Token);

    private void FillControls()
    {
        Version = $"Версия: {featureCollection.Helper.VersionProvider.Version}";

        Settings = featureCollection.Helper.SettingsProvider.GetSettings();
        DefaultParticipantName = Settings.DefaultParticipantName;
        LightThemeEnabled = Settings.LightThemeEnabled;
        DiscoveryTimeout = Settings.DiscoveryTimeout;
        DiscoveryPort = Settings.DiscoveryPort;
    }

    private bool CanSave() => !HasErrors;
}
