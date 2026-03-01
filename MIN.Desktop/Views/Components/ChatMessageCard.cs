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

            ResizeOutOfPrefferedSize();

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
            sendTime.Font = FontScheme.Monospace;
            sendMessage.Font = FontScheme.Default;
        }

        public void ResizeOutOfPrefferedSize()
        {
            Height = Convert.ToInt32(tableLayoutPanelLabels.RowStyles[0].Height)
               + sendMessage.PreferredSize.Height
               + sendMessage.Margin.Vertical * 4;

            Width = Convert.ToInt32(tableLayoutPanelLabels.ColumnStyles[1].Width)
                + Math.Max(sendMessage.PreferredSize.Width, senderName.PreferredSize.Width)
                + sendMessage.Padding.Horizontal
                + sendMessage.Margin.Horizontal;
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