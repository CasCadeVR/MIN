using MIN.Desktop.Contracts.Enums;
using MIN.Desktop.Contracts.Interfaces;
using MIN.Desktop.Contracts.Schemes;
using MIN.Desktop.Contracts.Views.PanelViews;
using MIN.Desktop.Views.Forms.HelperForms;
using MIN.DI.FeatureCollection;
using MIN.Helpers.Contracts.Models;

namespace MIN.Desktop.Views.Panels.SidePanelViews;

/// <summary>
/// Боковая панель настроек
/// </summary>
public partial class SettingsSidePanelView : StyledPanelView
{
    private readonly IMinFeatureCollection featureCollection;
    private readonly INavigationService navigationService;

    /// <summary>
    /// Текущие настройки
    /// </summary>
    public Settings Settings { get; set; } = null!;

    /// <inheritdoc />
    public override PanelType PanelType => PanelType.Side;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="MainSidePanelView"/>
    /// </summary>
    public SettingsSidePanelView(IMinFeatureCollection featureCollection,
        INavigationService navigationService)
    {
        InitializeComponent();

        this.featureCollection = featureCollection;
        this.navigationService = navigationService;
        featureCollection.Helper.SettingsProvider.OnSettingsSaved += FillControls;

        FillControls();
    }

    private void FillControls()
    {
        Settings = featureCollection.Helper.SettingsProvider.GetSettings();
        labelVersion.Text = $"Версия: {featureCollection.Helper.VersionProvider.Version.ToString()}";
        defaultName.Text = Settings.DefaultParticipantName;
        roomSearchTime.Value = Settings.DiscoveryTimeout;
        discoveryPort.Value = Settings.DiscoveryPort;
    }

    /// <inheritdoc />
    protected override void ApplyStylings()
    {
        splitContainer.Panel1.BackColor = ColorScheme.PrimaryAccent;
        splitContainer.Panel2.BackColor = ColorScheme.MainPanelBackground;
        Title.ForeColor = ColorScheme.TextOnAccent;
    }

    private void saveButton_Click(object sender, EventArgs e)
    {
        Settings.DefaultParticipantName = defaultName.Text;
        Settings.DiscoveryPort = Convert.ToInt32(discoveryPort.Value);
        Settings.DiscoveryTimeout = Convert.ToInt32(roomSearchTime.Value);
        featureCollection.Helper.SettingsProvider.SaveSettings(Settings);

        navigationService.NavigateTo<MainSidePanelView>();
    }

    private void logButton_Click(object sender, EventArgs e)
    {
        new LogForm(featureCollection.Helper.Logger).Show();
        navigationService.NavigateTo<MainSidePanelView>();
    }

    private void clearCacheButton_Click(object sender, EventArgs e)
    {
        featureCollection.Helper.AppDataProvider.ClearFolder("cryptography");
        featureCollection.Helper.AppDataProvider.ClearFolder("network");
    }
}
