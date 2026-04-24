using Microsoft.Extensions.DependencyInjection;
using MIN.Desktop.Contracts.Interfaces;
using MIN.Desktop.Contracts.Views.Forms;
using MIN.Desktop.Contracts.Views.PanelViews;
using MIN.Desktop.Contracts.Views.PanelViews.Models;

namespace MIN.Desktop.Infrastructure.Services;

/// <inheritdoc cref="INavigationService"/>
public class NavigationService : INavigationService
{
    BaseForm INavigationService.Parent { get; set; } = null!;

    /// <inheritdoc />
    public SplitContainer SplitContainer { get; set; } = null!;

    /// <inheritdoc />
    public Panel MainPanel { get; set; } = null!;

    /// <inheritdoc />
    public Panel SidePanel { get; set; } = null!;

    private readonly IServiceProvider provider;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="NavigationService"/>
    /// </summary>
    public NavigationService(IServiceProvider provider)
    {
        this.provider = provider;
    }

    void INavigationService.NavigateTo<TPanel>()
    {
        var panel = provider.GetRequiredService<TPanel>();
        NavigateToPanel(panel);
    }

    void INavigationService.NavigateTo<TPanel, TParams>(TParams param)
    {
        var panel = provider.GetRequiredService<TPanel>();

        if (panel is IPanelInitializeDepended<TParams> initializer)
        {
            initializer.Initialize(param);
        }

        NavigateToPanel(panel);
    }

    private void NavigateToPanel(BasePanelView panel)
    {
        switch (panel.PanelType)
        {
            case Contracts.Enums.PanelType.Main:
                MainPanel.Controls.Clear();
                SplitContainer.Panel2MinSize = panel.MinimumSize.Width;
                MainPanel.Controls.Add(panel);
                break;

            case Contracts.Enums.PanelType.Side:
                SidePanel.Controls.Clear();
                SplitContainer.Panel1MinSize = panel.MinimumSize.Width;
                SidePanel.Controls.Add(panel);
                break;
        }

        panel.Dock = DockStyle.Fill;
    }
}
