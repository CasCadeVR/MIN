using System.ComponentModel;
using MIN.Desktop.Contracts.Schemes;

namespace MIN.Desktop.Components.Controls.MaskedTextBoxes;

/// <summary>
/// Базовый <see cref="MaskedTextBox"/>
/// </summary>
[DefaultProperty("Text")]
[ToolboxItem(true)]
[DesignerCategory("Code")]
public class BaseMaskedTextBox : MaskedTextBox
{
    /// <summary>
    /// Инициализирует новый экземпляр <see cref="BaseMaskedTextBox"/>
    /// </summary>
    public BaseMaskedTextBox()
    {
        ApplyBaseStyles();
    }

    /// <summary>
    /// Наложить базовые стили
    /// </summary>
    protected virtual void ApplyBaseStyles()
    {
        Font = FontScheme.Default;
        ForeColor = ColorScheme.TextPrimary;
        BackColor = ColorScheme.InputFieldBackground;
        BorderStyle = BorderStyle.FixedSingle;
    }
}
