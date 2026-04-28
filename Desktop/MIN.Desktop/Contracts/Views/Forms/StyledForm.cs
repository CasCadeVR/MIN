using MIN.Desktop.Contracts.Interfaces;
using MIN.Desktop.Contracts.Schemes;
using MIN.Desktop.Properties;

namespace MIN.Desktop.Contracts.Views.Forms;

/// <summary>
/// Стилизированная форма
/// </summary>
public partial class StyledForm : BaseForm, IStyled
{
    /// <summary>
    /// Инициализирует новый экземпляр <see cref="BaseForm"/>
    /// </summary>
    public StyledForm()
    {
        InitializeComponent();
        Icon = Resources.logo;
    }

    /// <inheritdoc cref="Form.OnLoad(EventArgs)"/>
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        ApplyStylings();
    }

    void IStyled.ApplyStylings()
    {
        ApplyStylings();
    }

    /// <inheritdoc cref="IStyled.ApplyStylings"/>
    protected virtual void ApplyStylings()
    {
        BackColor = ColorScheme.FormBackground;
    }
}
