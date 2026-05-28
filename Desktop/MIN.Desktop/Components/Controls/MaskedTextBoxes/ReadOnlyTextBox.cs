using MIN.Desktop.Contracts.Schemes;

namespace MIN.Desktop.Components.Controls.MaskedTextBoxes
{
    /// <summary>
    /// Textbox только для чтения от <see cref="BaseMaskedTextBox" />
    /// </summary>
    public class ReadOnlyMaskedTextBox : BaseMaskedTextBox
    {
        /// <summary>
        /// Инициализирует новый экземпляр <see cref="ReadOnlyMaskedTextBox"/>
        /// </summary>
        public ReadOnlyMaskedTextBox()
        {
            ReadOnly = true;
        }

        /// <inheritdoc cref="BaseMaskedTextBox.ApplyBaseStyles" />
        protected override void ApplyBaseStyles()
        {
            base.ApplyBaseStyles();
            ReadOnly = true;
            BackColor = ColorScheme.MainPanelBackground;
            ForeColor = ColorScheme.TextPrimary;
            Font = FontScheme.Monospace;
            BorderStyle = BorderStyle.None;
        }
    }
}
