using MIN.Desktop.Contracts.Schemes;

namespace MIN.Desktop.Components.Controls.TextBoxes
{
    /// <summary>
    /// Обычный <see cref="BaseTextBox"/>
    /// </summary>
    public class DefaultTextBox : BaseTextBox
    {
        /// <inheritdoc cref="BaseTextBox.ApplyBaseStyles"/>
        protected override void ApplyBaseStyles()
        {
            base.ApplyBaseStyles();
            BorderStyle = BorderStyle.None;
            ForeColor = ColorScheme.TextSecondary;
        }
    }
}
