using MIN.Desktop.Contracts;

namespace MIN.Desktop.Components.Controls.CheckBoxes
{
    public class DefaultCheckBox : BaseCheckBox
    {
        public DefaultCheckBox()
        {
            ApplyBaseStyles();
        }

        protected override void ApplyBaseStyles() => BackColor = ColorScheme.PrimaryAccent;
    }
}
