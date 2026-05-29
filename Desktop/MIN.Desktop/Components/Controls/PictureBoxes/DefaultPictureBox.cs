using MIN.Desktop.Components.Controls.RadioButtons;
using MIN.Desktop.Contracts.Schemes;

namespace MIN.Desktop.Components.Controls.PictureBoxes
{
    /// <summary>
    /// Обычный <see cref="BasePictureBox"/>
    /// </summary>
    public class DefaultPictureBox : BasePictureBox
    {
        /// <summary>
        /// Инциализирует новый экземпляр <see cref="DefaultPictureBox"/>
        /// </summary>
        public DefaultPictureBox()
        {
            ApplyBaseStyles();
        }

        /// <inheritdoc cref="BaseRadioButton.ApplyBaseStyles"/>
        protected override void ApplyBaseStyles()
        {
            BackColor = ColorScheme.InputFieldBackground;
            SizeMode = PictureBoxSizeMode.Zoom;
        }
    }
}
