using System.ComponentModel;
using MIN.Desktop.Contracts;

namespace MIN.Desktop.Components.Controls.NumericUpDowns
{
    /// <summary>
    /// Базовый <see cref="NumericUpDown"/>
    /// </summary>
    [ToolboxItem(true)]
    [DesignerCategory("Code")]
    public class BaseNumericUpDown : NumericUpDown
    {
        /// <summary>
        /// Инциализирует новый экземпляр <see cref="BaseNumericUpDown"/>
        /// </summary>
        public BaseNumericUpDown()
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
