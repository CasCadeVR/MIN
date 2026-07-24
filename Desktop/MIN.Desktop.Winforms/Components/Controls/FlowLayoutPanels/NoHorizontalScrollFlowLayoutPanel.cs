using System.Runtime.InteropServices;

namespace MIN.Desktop.Components.Controls.FlowLayoutPanels
{
    /// <summary>
    /// <see cref="FlowLayoutPanel"/>, но без горизонтального скролла
    /// </summary>
    public class NoHorizontalScrollListView : FlowLayoutPanel
    {
        private const int GWL_STYLE = -16;
        private const int WS_HSCROLL = 0x00100000;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        /// <summary>
        /// Скрыть горизонтальный скролл процессорно
        /// </summary>
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            var currentStylePtr = GetWindowLongPtr(Handle, GWL_STYLE);
            var currentStyle = currentStylePtr.ToInt32();

            if ((currentStyle & WS_HSCROLL) != 0)
            {
                var newStyle = currentStyle & ~WS_HSCROLL;
                SetWindowLongPtr(Handle, GWL_STYLE, new IntPtr(newStyle));

                Invalidate();
            }
        }
    }
}
