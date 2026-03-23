using System.ComponentModel;
using MIN.Desktop.Contracts;

namespace MIN.Desktop.Components.Controls.NumericUpDowns
{
    [ToolboxItem(true)]
    [DesignerCategory("Code")]
    public class BaseNumericUpDown : NumericUpDown
    {
        public BaseNumericUpDown()
        {
            ApplyBaseStyles();
        }

        protected virtual void ApplyBaseStyles()
        {
            Font = FontScheme.Default;
            ForeColor = ColorScheme.TextPrimary;
            BackColor = ColorScheme.InputFieldBackground;
        }
    }
}
