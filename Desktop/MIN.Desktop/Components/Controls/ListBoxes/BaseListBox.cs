using MIN.Desktop.Contracts;

namespace MIN.Desktop.Components.Controls.ListBoxes
{
    /// <summary>
    /// Базовый <see cref="ListBox"/>
    /// </summary>
    public class BaseListBox : ListBox
    {
        /// <summary>
        /// Инциализирует новый экземпляр <see cref="BaseListBox"/>
        /// </summary>
        public BaseListBox()
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
            BackColor = ColorScheme.InputFieldBackground;
        }
    }
}
