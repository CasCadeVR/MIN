using System.ComponentModel;
using MIN.Desktop.Contracts.Schemes;

namespace MIN.Desktop.Components.Controls.Buttons
{
    /// <summary>
    /// Обычная <see cref="Button"/>
    /// </summary>
    [DefaultEvent("Click")]
    public class CommonButton : Button
    {
        /// <summary>
        /// Инициализирует новый экземпляр <see cref="CommonButton"/>
        /// </summary>
        public CommonButton()
        {
            ApplyAppStyles();
        }

        private void ApplyAppStyles()
        {
            base.Font = FontScheme.Emphasis;

            base.BackColor = ColorScheme.SecondaryAccent;
            base.ForeColor = ColorScheme.InputFieldBackground;

            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderColor = ColorScheme.DividerColor;
            FlatAppearance.BorderSize = 1;
        }

        /// <inheritdoc cref="Control.OnHandleCreated(EventArgs)"/>
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            if (!DesignMode)
            {
                ApplyAppStyles();
            }
        }
    }
}
