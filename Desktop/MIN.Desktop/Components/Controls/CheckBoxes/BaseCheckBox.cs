using MIN.Desktop.Contracts;

namespace MIN.Desktop.Components.Controls.CheckBoxes
{
    public class BaseCheckBox : CheckBox
    {
        public BaseCheckBox()
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
