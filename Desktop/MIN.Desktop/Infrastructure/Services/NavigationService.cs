using Microsoft.Extensions.DependencyInjection;
using MIN.Desktop.Contracts.Interfaces;
using MIN.Desktop.Contracts.Views.Forms;
using MIN.Desktop.Contracts.Views.PanelViews;
using MIN.Desktop.Contracts.Views.PanelViews.Interfaces;

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

    /// <inheritdoc />
    public SynchronizationContext UiContext { get; set; } = null!;

    private readonly IServiceProvider provider;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="NavigationService"/>
    /// </summary>
    public NavigationService(IServiceProvider provider)
    {
        this.provider = provider;
    }

    TPanel INavigationService.NavigateTo<TPanel>()
    {
        var panel = provider.GetRequiredService<TPanel>();
        NavigateToPanel(panel);
        return panel;
    }

    TPanel INavigationService.NavigateTo<TPanel, TParams>(TParams param)
    {
        var panel = provider.GetRequiredService<TPanel>();

        if (panel is IPanelInitializeDepended<TParams> initializer)
        {
            initializer.Initialize(param);
        }

        NavigateToPanel(panel);
        return panel;
    }

    TPanel INavigationService.NavigateToExisting<TPanel>(TPanel panel)
    {
        NavigateToPanel(panel);
        return panel;
    }

    private void NavigateToPanel(BasePanelView panel)
    {
        UiContext.Post(_ =>
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
        }, null);
    }
}
