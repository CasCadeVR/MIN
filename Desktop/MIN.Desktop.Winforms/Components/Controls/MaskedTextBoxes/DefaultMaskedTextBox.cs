using MIN.Desktop.Contracts.Schemes;

namespace MIN.Desktop.Components.Controls.MaskedTextBoxes
{
    /// <summary>
    /// Обычный <see cref="BaseMaskedTextBox"/>
    /// </summary>
    public class DefaultMaskedTextBox : BaseMaskedTextBox
    {
        /// <inheritdoc cref="BaseMaskedTextBox.ApplyBaseStyles" />
        protected override void ApplyBaseStyles()
        {
            base.ApplyBaseStyles();
            BackColor = ColorScheme.InputFieldBackground;
        }
    }
}
