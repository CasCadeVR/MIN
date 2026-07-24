using MIN.Desktop.Contracts.Schemes;

namespace MIN.Desktop.Components.Controls.CheckBoxes
{
    /// <summary>
    /// Обычный <see cref="BaseCheckBox"/>
    /// </summary>
    public class DefaultCheckBox : BaseCheckBox
    {
        /// <summary>
        /// Инциализирует новый экземпляр <see cref="BaseCheckBox"/>
        /// </summary>
        public DefaultCheckBox()
        {
            ApplyBaseStyles();
        }

        /// <inheritdoc cref="BaseCheckBox.ApplyBaseStyles"/>
        protected override void ApplyBaseStyles() => BackColor = ColorScheme.PrimaryAccent;
    }
}
