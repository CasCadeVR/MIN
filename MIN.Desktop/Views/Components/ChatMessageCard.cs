using MIN.Desktop.Contracts;
using MIN.Desktop.Infrastructure.Services;
using MIN.Services.Contracts.Models.Messages;

namespace MIN.Desktop.Components
{
    /// <summary>
    /// Кнопка меню
    /// </summary>
    public partial class ChatMessageCard : UserControl
    {
        private readonly ChatMessage chatMessage;
        private readonly bool hostMessage;
        private readonly bool removeHeaders;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="RoomCard"/>
        /// </summary>
        public ChatMessageCard(ChatMessage chatMessage, bool hostMessage, bool removeHeaders)
        {
            InitializeComponent();
            this.chatMessage = chatMessage;
            this.hostMessage = hostMessage;
            this.removeHeaders = removeHeaders;
            FillLabels();
            ApplyStylings();
        }

        private void ApplyStylings()
        {
            if (removeHeaders)
            {
                tableLayoutPanelLabels.RowStyles[0].Height = 0;
                senderName.Visible = false;
                sendRole.Visible = false;
            }

            var senderColor = chatMessage.SenderPCName == AppUserProvider.Instance.CurrentUser.PCName
                ? ColorScheme.OutgoingMessageBackground
                : ColorScheme.IncomingMessageBackground;

            senderName.BackColor = senderColor;
            sendRole.BackColor = senderColor;
            sendMessage.BackColor = senderColor;
            sendTime.BackColor = senderColor;
            tableLayoutPanelLabels.BackColor = senderColor;

            senderName.Font = FontScheme.Monospace;
            sendRole.Font = FontScheme.Monospace;
            sendTime.Font = FontScheme.Caption;
            sendMessage.Font = FontScheme.Default;
        }

        /// <summary>
        /// Подстроивает размеры сообщений под текст внутри и возвращает высоту
        /// </summary>
        public int ResizeOutOfPrefferedSize()
        {
            var wantedWidth = Math.Min(Convert.ToInt32(Parent!.Width * 0.7),
                Convert.ToInt32(tableLayoutPanelLabels.ColumnStyles[1].Width)
                + Math.Max(sendMessage.PreferredSize.Width, senderName.PreferredSize.Width)
                + sendMessage.Margin.Horizontal * 2);

            if (Width == wantedWidth)
            {
                return Height;
            }

            Width = wantedWidth;

            var availableWidth = Width
                - sendMessage.Margin.Horizontal * 2
                - Convert.ToInt32(tableLayoutPanelLabels.ColumnStyles[1].Width);

            var lineCount = CalculateLineCount(sendMessage, availableWidth);

            var gottenHeight = Convert.ToInt32(tableLayoutPanelLabels.RowStyles[0].Height)
                + (lineCount * sendMessage.Font.Height)
                + sendMessage.Margin.Vertical * 2;

            Height = gottenHeight;
            return gottenHeight;
        }

        private static int CalculateLineCount(TextBox textBox, int availableWidth)
        {
            var originalWidth = textBox.Width;
            textBox.Width = availableWidth;

            var lastCharLine = textBox.GetLineFromCharIndex(textBox.Text.Length - 1);
            var lines = lastCharLine + 1;

            textBox.Width = originalWidth;
            return Math.Max(1, lines);
        }

        private void FillLabels()
        {
            senderName.Text = chatMessage.SenderName;
            sendRole.Text = hostMessage ? "Хост" : string.Empty;
            sendTime.Text = chatMessage.Time.ToShortTimeString();
            sendMessage.Text = chatMessage.Content;
        }
    }
}
