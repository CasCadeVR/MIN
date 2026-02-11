using MIN.Desktop.Contracts;
using MIN.Desktop.Infrastructure.Services;
using MIN.Services.Contracts.Models;

namespace MIN.Desktop.Components
{
    /// <summary>
    /// Кнопка меню
    /// </summary>
    public partial class ChatMessageCard : UserControl
    {
        private const int MaxLines = 100;            // ограничение, чтобы не было "монстров"
        private readonly ChatMessage chatMessage;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="RoomCard"/>
        /// </summary>
        public ChatMessageCard(ChatMessage chatMessage)
        {
            InitializeComponent();
            this.chatMessage = chatMessage;
            ApplyStylings();
            FillLabels();
        }

        private void ApplyStylings()
        {
            int lineCount = CalculateLineCount(chatMessage.Content);
            int contentHeight = lineCount * sendMessage.Height;
            int totalHeight = sendMessage.Height + contentHeight + sendMessage.Height + sendMessage.Padding.Top;

            Height = totalHeight;

            tableLayoutPanelLabels.BackColor =
                chatMessage.SenderPCName == AppUserProvider.Instance.CurrentUser.PCName
                    ? ColorScheme.OutgoingMessageBackground
                    : ColorScheme.IncomingMessageBackground;

            this.Width = this.Parent?.Width ?? 400;
        }

        private int CalculateLineCount(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return 1;

            using var graphics = CreateGraphics();
            var font = sendMessage.Font;

            var rect = new RectangleF(0, 0, Width, 1000);
            var flags = TextFormatFlags.WordBreak | TextFormatFlags.TextBoxControl;
            var size = TextRenderer.MeasureText(graphics, text, font, new Size(Width, int.MaxValue), flags);

            int lines = Math.Max(1, (int)Math.Ceiling((double)size.Height / sendMessage.Height));
            return Math.Min(lines, MaxLines);
        }

        private void FillLabels()
        {
            senderName.Text = chatMessage.SenderName;
            sendRole.Text = chatMessage.SenderPCName == AppUserProvider.Instance.CurrentUser.PCName ? "Хост" : string.Empty;
            sendTime.Text = chatMessage.Time.ToShortTimeString();
            sendMessage.Text = chatMessage.Content;
        }
    }
}