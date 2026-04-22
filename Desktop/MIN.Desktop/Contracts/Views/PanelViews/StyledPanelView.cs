using MIN.Desktop.Contracts.Interfaces;
using MIN.Desktop.Contracts.Models;
using MIN.Desktop.Contracts.Schemes;

namespace MIN.Desktop.Contracts.Views.PanelViews;

/// <summary>
/// Стилизированная вкладка
/// </summary>
public partial class StyledPanelView : BasePanelView, IStyled
{
    /// <summary>
    /// Инициализирует новый экземляр <see cref="BasePanelView"/>
    /// </summary>
    public StyledPanelView()
    {
        InitializeComponent();
        //ApplyStylings();
    }

    /// <summary>
    /// Инициализирует новый экземляр <see cref="BasePanelView"/>
    /// </summary>
    public StyledPanelView(NavigationItem item)
    {
        InitializeComponent();
        ApplyStylings();
    }

    /// <inheritdoc />
    public virtual void ApplyStylings()
    {
        splitContainer.BackColor = ColorScheme.MainPanelBackground;
        splitContainer.ForeColor = ColorScheme.TextPrimary;
    }
}
