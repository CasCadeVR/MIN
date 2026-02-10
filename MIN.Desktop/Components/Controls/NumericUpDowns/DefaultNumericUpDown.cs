using MIN.Desktop.Contracts;

namespace MIN.Desktop.Components.Controls.NumericUpDowns
{
    /// <summary>
    /// Обычный <see cref="BaseNumericUpDown">
    /// </summary>
    public class DefaultNumericUpDown : BaseNumericUpDown
    {
        protected override void ApplyBaseStyles()
        {
            base.ApplyBaseStyles();
            BorderStyle = BorderStyle.None;
            ForeColor = ColorScheme.TextSecondary;
        }
    }
}
