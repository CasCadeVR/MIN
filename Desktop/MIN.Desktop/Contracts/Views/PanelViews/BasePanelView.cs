using MIN.Desktop.Contracts.Enums;
using MIN.Desktop.Contracts.Views.PanelViews.Models;

namespace MIN.Desktop.Contracts.Views.PanelViews;

/// <summary>
/// Базовая вкладка
/// </summary>
public partial class BasePanelView : UserControl, IPanel
{
    /// <inheritdoc />
    public virtual PanelType PanelType { get; }

    /// <summary>
    /// Инициализирует новый экземляр <see cref="BasePanelView"/>
    /// </summary>
    public BasePanelView()
    {
        InitializeComponent();
    }

    /// <inheritdoc />
    public virtual void OnNavigatedTo() => throw new NotImplementedException();
}
