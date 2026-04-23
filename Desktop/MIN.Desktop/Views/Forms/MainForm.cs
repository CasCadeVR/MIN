using MIN.Desktop.Contracts.Interfaces;
using MIN.Desktop.Contracts.Schemes;
using MIN.Desktop.Contracts.Views.Forms;
using MIN.Desktop.Views.Panels.SidePanelViews;

namespace MIN.Desktop;

/// <summary>
/// Главная форма приложения
/// </summary>
public partial class MainForm : StyledForm
{
    /// <summary>
    /// Инициализирует новый экземпляр <see cref="MainForm"/>
    /// </summary>
    public MainForm(INavigationService navigationService)
    {
        InitializeComponent();

        navigationService.NavigateTo<MainSidePanelView>();
        navigationService.NavigateTo<DiscoveryPanelView>();
    }

    /// <inheritdoc />
    protected override void ApplyStylings()
    {
        sidePanel.BackColor = ColorScheme.MainPanelBackground;
        mainPanel.BackColor = ColorScheme.FormBackground;
    }
}
