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
        private readonly ChatMessage chatMessage;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="RoomCard"/>
        /// </summary>
        public ChatMessageCard(ChatMessage chatMessage)
        {
            InitializeComponent();
            ApplyStylings();
            this.chatMessage = chatMessage;
            FillLabels();
        }

        private void ApplyStylings()
        {
            tableLayoutPanelLabels.BackColor = ColorScheme.IncomingMessageBackground;
            this.Width = this.Parent?.Width ?? 400;
        }

        private void FillLabels()
        {
            senderName.Text = chatMessage.SenderName;
            if (chatMessage.SenderPCName == AppUserProvider.Instance.CurrentUser.PCName)
            {
                sendRole.Text = "Хост";
            }
            else
            {
                sendRole.Text = "";
            }

            sendTime.Text = chatMessage.Time.ToShortTimeString();
            sendMessage.Text = chatMessage.Content;
        }
    }
}