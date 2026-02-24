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

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="RoomCard"/>
        /// </summary>
        public ChatMessageCard(ChatMessage chatMessage, bool hostMessage)
        {
            InitializeComponent();
            this.chatMessage = chatMessage;
            this.hostMessage = hostMessage;
            FillLabels();
            ApplyStylings();
        }

        private void ApplyStylings()
        {
            Height = senderName.Height
                + sendMessage.PreferredSize.Height
                + sendTime.Height
                + sendMessage.Padding.Vertical;

            Width = Convert.ToInt32(tableLayoutPanelLabels.ColumnStyles[1].Width)
                + Math.Max(sendMessage.PreferredSize.Width, senderName.PreferredSize.Width)
                + sendMessage.Padding.Horizontal
                + sendMessage.Margin.Horizontal;

            var color = chatMessage.SenderPCName == AppUserProvider.Instance.CurrentUser.PCName
             ? ColorScheme.OutgoingMessageBackground
             : ColorScheme.IncomingMessageBackground;

            senderName.BackColor = color;
            sendRole.BackColor = color;
            sendMessage.BackColor = color;
            sendTime.BackColor = color;
            tableLayoutPanelLabels.BackColor = color;
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