using MIN.Desktop.Contracts;

namespace MIN.Desktop.Components.Controls.RadioButtons
{
    /// <summary>
    /// Обычный <see cref="BaseRadioButton"/>
    /// </summary>
    public class DefaultRadioButton : BaseRadioButton
    {
        /// <summary>
        /// Инциализирует новый экземпляр <see cref="DefaultRadioButton"/>
        /// </summary>
        public DefaultRadioButton()
        {
            ApplyBaseStyles();
        }

        /// <inheritdoc cref="BaseRadioButton.ApplyBaseStyles"/>
        protected override void ApplyBaseStyles() => BackColor = ColorScheme.InputFieldBackground;
    }
}
