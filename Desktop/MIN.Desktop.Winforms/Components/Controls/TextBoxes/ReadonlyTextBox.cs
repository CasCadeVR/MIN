using MIN.Desktop.Components.Controls.TextBoxes;
using MIN.Desktop.Contracts.Schemes;
using System.Runtime.InteropServices;

namespace MIN.Desktop.Components.Textboxes
{
    /// <summary>
    /// Кастомный <see cref="TextBox"/> только для чтения без курсора
    /// </summary>
    public class ReadonlyTextbox : BaseTextBox
    {
        [DllImport("user32.dll")]
        static extern bool HideCaret(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="ReadonlyTextbox"/>
        /// </summary>
        public ReadonlyTextbox()
        {
            Font = FontScheme.Monospace;
            BorderStyle = BorderStyle.None;
            ForeColor = ColorScheme.TextPrimary;
            ReadOnly = true;
            GotFocus += (sender, ev) => { HideCaret(Handle); };
        }

        private const int WM_MOUSEWHEEL = 0x020A;

        /// <summary>
        /// Событие по прокрутке мыши
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            Control parent = Parent!;

            while (parent != null)
            {
                if (parent is ScrollableControl scrollableParent)
                {
                    var screenPoint = PointToScreen(new Point(e.X, e.Y));
                    var wParam = new IntPtr(e.Delta << 16);
                    var lParam = new IntPtr((screenPoint.Y << 16) | (screenPoint.X & 0xFFFF));

                    SendMessage(scrollableParent.Handle, WM_MOUSEWHEEL, wParam, lParam);
                    return;
                }
                parent = parent.Parent!;
            }

            base.OnMouseWheel(e);
        }

    }
}
