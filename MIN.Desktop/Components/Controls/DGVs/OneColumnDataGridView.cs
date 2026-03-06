using MIN.Desktop.Contracts;

namespace MIN.Desktop.Components.Controls.DGVs
{
    public class OneColumnDataGridView : BaseDataGridView
    {
        public OneColumnDataGridView()
        {
            AllowUserToAddRows = true;
            AllowUserToDeleteRows = true;
            AllowUserToOrderColumns = false;
            AllowUserToResizeColumns = false;
            AllowUserToResizeRows = false;
            ApplyBaseStyles();

            EnabledChanged += OneColumnDataGridView_EnabledChanged;
            OneColumnDataGridView_EnabledChanged(this, EventArgs.Empty);
        }

        private void OneColumnDataGridView_EnabledChanged(object? sender, EventArgs e)
        {
            BackgroundColor = Enabled ? ColorScheme.InputFieldBackground : ColorScheme.RoomFilled;
            DefaultCellStyle.BackColor = Enabled ? ColorScheme.PrimaryAccent : ColorScheme.RoomFilled;
        }

        protected virtual void ApplyBaseStyles()
        {
            base.ApplyBaseStyles();
            DefaultCellStyle.ForeColor = ColorScheme.TextOnAccent;
        }
    }
}
