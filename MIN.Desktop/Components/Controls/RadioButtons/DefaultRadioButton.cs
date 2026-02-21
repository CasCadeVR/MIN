using MIN.Desktop.Contracts;

namespace MIN.Desktop.Components.Controls.RadioButtons
{
    public class DefaultRadioButton : BaseRadioButton
    {
        public DefaultRadioButton()
        {
            ApplyBaseStyles();
        }

        protected virtual void ApplyBaseStyles()
        {
            BackColor = ColorScheme.InputFieldBackground;
        }
    }
}
