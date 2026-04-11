using System.ComponentModel;
using MIN.Desktop.Contracts;

namespace MIN.Desktop.Components.Controls.TextBoxes
{
    /// <summary>
    /// Базовый <see cref="TextBox"/>
    /// </summary>
    [DefaultProperty("Text")]
    [ToolboxItem(true)]
    [DesignerCategory("Code")]
    public class BaseTextBox : TextBox
    {
        /// <summary>
        /// Инциализирует новый экземпляр <see cref="BaseTextBox"/>
        /// </summary>
        public BaseTextBox()
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
        }
    }
}
