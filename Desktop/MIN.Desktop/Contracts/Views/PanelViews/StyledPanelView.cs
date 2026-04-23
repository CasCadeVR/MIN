using MIN.Desktop.Contracts.Interfaces;
using MIN.Desktop.Contracts.Schemes;

namespace MIN.Desktop.Contracts.Views.PanelViews;

/// <summary>
/// Стилизированная вкладка
/// </summary>
public partial class StyledPanelView : BasePanelView, IStyled
{
    /// <summary>
    /// Контекст синхронизации для UI
    /// </summary>
    protected SynchronizationContext uiContext;

    /// <summary>
    /// Инициализирует новый экземляр <see cref="BasePanelView"/>
    /// </summary>
    public StyledPanelView()
    {
        InitializeComponent();
        ApplyStylings();
        uiContext = SynchronizationContext.Current
            ?? throw new InvalidOperationException("Must be created on UI thread");
    }

    void IStyled.ApplyStylings()
    {
        ApplyStylings();
    }

    /// <inheritdoc cref="IStyled.ApplyStylings"/>
    protected virtual void ApplyStylings()
    {
        splitContainer.BackColor = ColorScheme.MainPanelBackground;
        splitContainer.ForeColor = ColorScheme.TextPrimary;
    }
}
