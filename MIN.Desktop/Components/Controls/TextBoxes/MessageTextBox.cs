namespace MIN.Desktop.Components.Controls.TextBoxes
{
    /// <summary>
    /// Текстовое поле для ввода сообщений с автоматическим изменением высоты (до 10 строк)
    /// </summary>
    public class MessageTextBox : DefaultTextBox
    {
        private const int MaxLines = 10;
        private const int LineHeight = 18; // под Segoe UI 9.75pt
        private const int MinVisibleLines = 1;
        private const int PaddingVertical = 10; // внутренние отступы

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="MessageTextBox"/>
        /// </summary>
        public MessageTextBox()
        {
            ApplyMessageStyles();
            TextChanged += OnTextChanged;
            UpdateHeight();
        }

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

        private void OnTextChanged(object? sender, EventArgs e)
        {
            UpdateHeight();
        }

        /// <summary>
        /// Обновить высоту на основе содержимого и доступной ширины.
        /// </summary>
        public void UpdateHeight()
        {
            if (IsDisposed || Disposing || Width <= 0) return;

            int lineCount = CountLines(Text);
            lineCount = Math.Max(MinVisibleLines, Math.Min(lineCount, MaxLines));

            int contentHeight = lineCount * LineHeight;
            int totalHeight = contentHeight + PaddingVertical;

            // Устанавливаем новую высоту
            Height = totalHeight;
        }

        private int CountLines(string text)
        {
            if (string.IsNullOrEmpty(text))
                return 1;

            using var g = CreateGraphics();
            var size = TextRenderer.MeasureText(
                g,
                text,
                Font,
                new Size(Width, int.MaxValue),
                TextFormatFlags.WordBreak | TextFormatFlags.TextBoxControl
            );
            return Math.Max(1, (int)Math.Ceiling((double)size.Height / LineHeight));
        }
    }
}