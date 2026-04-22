using MIN.Desktop.Contracts;
using MIN.Desktop.Contracts.Schemes;

namespace MIN.Desktop.Components.Controls.DGVs
{
    /// <summary>
    /// Базовый <see cref="DataGridView"/>
    /// </summary>
    public class BaseDataGridView : DataGridView
    {
        /// <summary>
        /// Инциализирует новый экземпляр <see cref="BaseDataGridView"/>
        /// </summary>
        public BaseDataGridView()
        {
            ApplyBaseStyles();
        }

        /// <summary>
        /// Наложить базовые стили
        /// </summary>
        protected virtual void ApplyBaseStyles()
        {
            Font = FontScheme.Caption;
            ForeColor = ColorScheme.TextPrimary;
            GridColor = ColorScheme.InputFieldBackground;
            BackgroundColor = ColorScheme.ChatAreaBackground;
        }
    }
}
