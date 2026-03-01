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

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            // Пытаемся найти родительский контейнер, который умеет скроллиться
            Control parent = this.Parent!;

            while (parent != null)
            {
                if (parent is ScrollableControl scrollableParent)
                {
                    // Если нашли родительский скролл-контроль (FlowLayoutPanel, Panel и т.д.),
                    // отправляем событие колесика ему.

                    // Координаты мыши должны быть экранными для SendMessage
                    Point screenPoint = this.PointToScreen(new Point(e.X, e.Y));
                    IntPtr wParam = new IntPtr((e.Delta << 16));
                    IntPtr lParam = new IntPtr((screenPoint.Y << 16) | (screenPoint.X & 0xFFFF));

                    SendMessage(scrollableParent.Handle, WM_MOUSEWHEEL, wParam, lParam);
                    return; // Выходим, чтобы TextBox не скроллил сам себя
                }
                parent = parent.Parent!;
            }

            // Если родителя со скроллом не нашли, оставляем стандартное поведение
            base.OnMouseWheel(e);
        }

    }
}