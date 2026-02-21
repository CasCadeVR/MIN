using MIN.Desktop.Contracts;

namespace MIN.Desktop.Components.Controls.DGVs
{
    public class BaseDataGridView : DataGridView
    {
        public BaseDataGridView()
        {
            ApplyBaseStyles();
        }

        protected virtual void ApplyBaseStyles()
        {
            Font = FontScheme.Caption;
            ForeColor = ColorScheme.TextPrimary;
            GridColor = ColorScheme.InputFieldBackground;
            BackgroundColor = ColorScheme.ChatAreaBackground;
        }
    }
}
