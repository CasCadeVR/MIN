using Microsoft.Extensions.DependencyInjection;
using MIN.Desktop.Contracts.Interfaces;
using MIN.Desktop.Contracts.Views.PanelViews;
using MIN.Desktop.Contracts.Views.PanelViews.Models;

namespace MIN.Desktop.Infrastructure.Services;

/// <inheritdoc cref="INavigationService"/>
public class NavigationService : INavigationService
{
    private readonly Panel mainPanel;
    private readonly Panel sidePanel;
    private readonly IServiceProvider provider;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="NavigationService"/>
    /// </summary>
    public NavigationService(Panel mainPanel, Panel sidePanel, IServiceProvider provider)
    {
        this.mainPanel = mainPanel;
        this.sidePanel = sidePanel;
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
                mainPanel.Controls.Clear();
                mainPanel.Controls.Add(panel);
                break;

            case Contracts.Enums.PanelType.Side:
                sidePanel.Controls.Clear();
                sidePanel.Controls.Add(panel);
                break;

        }

        panel.Dock = DockStyle.Fill;
    }
}
