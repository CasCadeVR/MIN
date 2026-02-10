using System.ComponentModel;
using MIN.Desktop.Contracts;

namespace MIN.Desktop.Components.Controls.TextBoxes
{
    [DefaultProperty("Text")]
    [ToolboxItem(true)]
    [DesignerCategory("Code")]
    public class BaseTextBox : TextBox
    {
        public BaseTextBox()
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
