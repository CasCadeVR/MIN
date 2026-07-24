using MIN.Desktop.Contracts.Schemes;

namespace MIN.Desktop.Components.Controls.RadioButtons
{
    /// <summary>
    /// Базовый <see cref="CheckBox"/>
    /// </summary>
    public class BaseRadioButton : RadioButton
    {
        /// <summary>
        /// Инциализирует новый экземпляр <see cref="BaseRadioButton"/>
        /// </summary>
        public BaseRadioButton()
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
