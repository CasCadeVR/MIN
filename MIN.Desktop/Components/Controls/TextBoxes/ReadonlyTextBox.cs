using MIN.Desktop.Components.Controls.TextBoxes;
using MIN.Desktop.Contracts;
using System.Runtime.InteropServices;

namespace MIN.Desktop.Components.Textboxes
{
    /// <summary>
    /// Кастомный <see cref="TextBox"/> только для чтения без курсора
    /// </summary>
    public class ReadonlyTextbox : BaseTextBox
    {
        /// <summary>
        /// Инициализирует новый экземпляр <see cref="ReadonlyTextbox"/>
        /// </summary>
        public ReadonlyTextbox()
        {
            Font = FontScheme.Heading3;
            BorderStyle = BorderStyle.None;
            ForeColor = ColorScheme.TextPrimary;
            ReadOnly = true;
            GotFocus += (sender, ev) => { HideCaret(Handle); };
        }

        [DllImport("user32.dll")]
        static extern bool HideCaret(IntPtr hWnd);
    }
}