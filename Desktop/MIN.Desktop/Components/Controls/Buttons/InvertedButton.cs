using System.ComponentModel;
using MIN.Desktop.Contracts;

namespace MIN.Desktop.Components.Controls.Buttons
{
    /// <summary>
    /// Обычная <see cref="Button"/>
    /// </summary>
    [DefaultEvent("Click")]
    public class InvertedButton : Button
    {
        /// <summary>
        /// Инициализирует новый экземпляр <see cref="InvertedButton"/>
        /// </summary>
        public InvertedButton()
        {
            ApplyAppStyles();
        }

        private void ApplyAppStyles()
        {
            base.Font = FontScheme.Emphasis;

            base.BackColor = ColorScheme.InputFieldBackground;
            base.ForeColor = ColorScheme.SecondaryAccent;

            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderColor = ColorScheme.SecondaryAccent;
            FlatAppearance.BorderSize = 2;
            Padding = new Padding(8, 4, 8, 4);
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
