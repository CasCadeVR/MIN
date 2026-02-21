using MIN.Desktop.Contracts;

namespace MIN.Desktop.Components.Controls.RadioButtons
{
    public class BaseRadioButton : RadioButton
    {
        public BaseRadioButton()
        {
            ApplyBaseStyles();
        }

        protected virtual void ApplyBaseStyles()
        {
            Font = FontScheme.Default;
            ForeColor = ColorScheme.TextPrimary;
        }
    }
}
