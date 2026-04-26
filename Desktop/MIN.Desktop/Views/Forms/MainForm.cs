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

        navigationService.Parent = this;
        navigationService.SplitContainer = splitContainer;
        navigationService.SidePanel = splitContainer.Panel1;
        navigationService.MainPanel = splitContainer.Panel2;

        navigationService.UiContext = SynchronizationContext.Current
            ?? throw new InvalidOperationException("Must be created on UI thread");

        navigationService.NavigateTo<MainSidePanelView>();
        navigationService.NavigateTo<DiscoveryPanelView>();
    }

    /// <inheritdoc />
    protected override void ApplyStylings()
    {
        BackColor = ColorScheme.FormBackground;
    }
}
