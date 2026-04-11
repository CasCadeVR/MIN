using MIN.Desktop.Contracts;

namespace MIN.Desktop.Components.Controls.CheckBoxes
{
    /// <summary>
    /// Базовый <see cref="CheckBox"/>
    /// </summary>
    public class BaseCheckBox : CheckBox
    {
        /// <summary>
        /// Инциализирует новый экземпляр <see cref="BaseCheckBox"/>
        /// </summary>
        public BaseCheckBox()
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
        }
    }
}
