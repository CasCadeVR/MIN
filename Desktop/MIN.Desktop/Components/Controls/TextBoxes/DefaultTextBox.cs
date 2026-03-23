using MIN.Desktop.Contracts;

namespace MIN.Desktop.Components.Controls.TextBoxes
{
    /// <summary>
    /// Обычный <see cref="BaseTextBox">
    /// </summary>
    public class DefaultTextBox : BaseTextBox
    {
        protected override void ApplyBaseStyles()
        {
            base.ApplyBaseStyles();
            BorderStyle = BorderStyle.None;
            ForeColor = ColorScheme.TextSecondary;
        }
    }
}
