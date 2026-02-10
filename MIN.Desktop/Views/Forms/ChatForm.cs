using System.Windows.Forms;
using MIN.Desktop.Components;
using MIN.Desktop.Contracts;
using MIN.Desktop.Contracts.Views.Forms;
using MIN.Desktop.Infrastructure.Services;
using MIN.Desktop.Views.Components;
using MIN.Services.Contracts.Models;
using MIN.Services.Contracts.Models.Enums;

namespace MIN.Desktop
{
    public partial class ChatForm : StyledForm
    {
        /// <summary>
        /// Текущая комната
        /// </summary>
        public Room Room { get; set; }

        public ChatForm(Room? room = null)
        {
            InitializeComponent();
            Room = room ?? new Room();
            Title.Text = $"Комната {Room.Name}";
        }

        protected override void ApplyStylings()
        {
            splitContainer.Panel1.BackColor = ColorScheme.PrimaryAccent;
            splitContainer.Panel2.BackColor = ColorScheme.MainPanelBackground;
            chatFlow.BackColor = ColorScheme.FormBackground;
        }

        private bool IsMessageValid(ChatMessage message)
        {
            return !string.IsNullOrEmpty(message.Content);
        }

        private void UpdateChatFlow()
        {
            chatFlow.Controls.Clear();

            Room.ChatHistory = Room.ChatHistory.OrderByDescending(x => x.Time).ToList();

            foreach (var message in Room.ChatHistory)
            {
                var row = new ChatMessageRow();

                var card = new ChatMessageCard(message) { Parent = row.container };

                if (message.SenderPCName == AppUserProvider.Instance.CurrentUser.PCName)
                {
                    card.Anchor = AnchorStyles.Right;
                }
                else
                {
                    card.Anchor = AnchorStyles.Left;
                }

                row.Width = chatFlow.Width - (row.Margin.Left * 2) - chatFlow.Margin.Left;
                chatFlow.Controls.Add(row);
            }
        }

        private void sendButton_Click(object sender, EventArgs e)
        {
            var message = new ChatMessage()
            {
                SenderName = AppUserProvider.Instance.CurrentUser.Name,
                SenderPCName = AppUserProvider.Instance.CurrentUser.PCName,
                Content = messageTextBox.Text,
                MessageType = MessageType.Text,
            };

            Room.ChatHistory.Add(message);

            if (!IsMessageValid(message))
            {
                return;
            }

            messageTextBox.Text = string.Empty;
            UpdateChatFlow();
        }

        private void disconnectButton_Click(object sender, EventArgs e)
        {
            // TODO: disconnect client somehow
            this.Close();
        }

        private void aboutButton_Click(object sender, EventArgs e)
        {

        }

        private void chatFlow_Resize(object sender, EventArgs e)
        {
            foreach (ChatMessageRow control in chatFlow.Controls)
            {
                control.Width = chatFlow.Width - (control.Margin.Left * 2) - chatFlow.Margin.Left;
            }
        }
    }
}
