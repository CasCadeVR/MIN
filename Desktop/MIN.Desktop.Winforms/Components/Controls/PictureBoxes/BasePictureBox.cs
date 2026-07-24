using MIN.Desktop.Contracts.Schemes;

namespace MIN.Desktop.Components.Controls.PictureBoxes
{
    /// <summary>
    /// Базовый <see cref="PictureBox"/>
    /// </summary>
    public class BasePictureBox : PictureBox
    {
        /// <summary>
        /// Инциализирует новый экземпляр <see cref="BasePictureBox"/>
        /// </summary>
        public BasePictureBox()
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
