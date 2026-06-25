namespace MIN.Desktop.Components.Controls.TextBoxes
{
    /// <summary>
    /// Текстовое поле для ввода сообщений с автоматическим изменением высоты (до 10 строк)
    /// </summary>
    public class MessageTextBox : DefaultTextBox
    {
        private const int MaxRowsHeight = 10;
        private const int MinHeightpx = 36;
        private const int PaddingVertical = 10; // внутренние отступы
        private int MaxHeightpx => Convert.ToInt32(Font.Height) * MaxRowsHeight;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="MessageTextBox"/>
        /// </summary>
        public MessageTextBox()
        {
            ApplyMessageStyles();
            UpdateHeight();
        }

        /// <inheritdoc cref="DefaultTextBox.ApplyBaseStyles"/>
        protected override void ApplyBaseStyles()
        {
            base.ApplyBaseStyles();
            ApplyMessageStyles();
        }

        private void ApplyMessageStyles()
        {
            BorderStyle = BorderStyle.None;
            BackColor = Color.FromArgb(248, 249, 255);
            ForeColor = Color.FromArgb(122, 119, 143);
            Font = new Font("Segoe UI", 9.75F);
            Multiline = true;
            ScrollBars = ScrollBars.Vertical;
            AcceptsReturn = true;
            PlaceholderText = "Message";
        }

        /// <summary>
        /// Обновить высоту на основе содержимого и доступной ширины
        /// </summary>
        public int UpdateHeight()
        {
            var lineCount = CalculateLineCount();

            var gottenHeight = (lineCount * Font.Height) + Margin.Vertical * 2;

            var totalHeight = Math.Max(Math.Min(gottenHeight, MaxHeightpx), MinHeightpx) + PaddingVertical;

            return totalHeight;
        }

        private int CalculateLineCount()
        {
            var lastCharLine = GetLineFromCharIndex(Text.Length - 1);
            var lines = lastCharLine + 1;

            return Math.Max(1, lines);
        }
    }
}
