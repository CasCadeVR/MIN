using MIN.Desktop.Contracts.Schemes;

namespace MIN.Desktop.Components.Controls.DGVs
{
    /// <summary>
    /// Одноколонный <see cref="BaseDataGridView"/>
    /// </summary>
    public class OneColumnDataGridView : BaseDataGridView
    {
        /// <summary>
        /// Инциализирует новый экземпляр <see cref="OneColumnDataGridView"/>
        /// </summary>
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
            BackgroundColor = Enabled ? ColorScheme.InputFieldBackground : ColorScheme.ConnectionDisabled;
            DefaultCellStyle.BackColor = Enabled ? ColorScheme.PrimaryAccent : ColorScheme.ConnectionDisabled;
        }

        /// <inheritdoc cref="BaseDataGridView.ApplyBaseStyles"/>
        protected override void ApplyBaseStyles()
        {
            base.ApplyBaseStyles();
            DefaultCellStyle.ForeColor = ColorScheme.TextOnAccent;
        }
    }
}
