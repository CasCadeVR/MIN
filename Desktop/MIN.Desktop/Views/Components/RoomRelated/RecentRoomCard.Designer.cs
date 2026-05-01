using MIN.Desktop.Components.Controls.Buttons;

namespace MIN.Desktop.Components
{
    partial class RecentRoomCard
    {
        /// <summary> 
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            backgroundPanel = new TableLayoutPanel();
            Title = new MIN.Desktop.Components.Labels.Heading1Label();
            lastMessageTime = new MIN.Desktop.Components.Labels.CaptionLabel();
            lastMessageSenderAndContent = new MIN.Desktop.Components.Labels.CaptionLabel();
            participantsInfo = new MIN.Desktop.Components.Labels.Heading3Label();
            missedMessagesCountLabel = new MIN.Desktop.Components.Labels.Heading3Label();
            backgroundPanel.SuspendLayout();
            SuspendLayout();
            // 
            // backgroundPanel
            // 
            backgroundPanel.BackColor = Color.Transparent;
            backgroundPanel.ColumnCount = 4;
            backgroundPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 87F));
            backgroundPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            backgroundPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 24F));
            backgroundPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 50F));
            backgroundPanel.Controls.Add(Title, 0, 0);
            backgroundPanel.Controls.Add(lastMessageTime, 3, 0);
            backgroundPanel.Controls.Add(lastMessageSenderAndContent, 0, 1);
            backgroundPanel.Controls.Add(participantsInfo, 2, 0);
            backgroundPanel.Controls.Add(missedMessagesCountLabel, 3, 1);
            backgroundPanel.Dock = DockStyle.Fill;
            backgroundPanel.Location = new Point(0, 0);
            backgroundPanel.Margin = new Padding(0);
            backgroundPanel.Name = "backgroundPanel";
            backgroundPanel.RowCount = 2;
            backgroundPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50.0003166F));
            backgroundPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 49.99968F));
            backgroundPanel.Size = new Size(343, 50);
            backgroundPanel.TabIndex = 2;
            backgroundPanel.Click += RecentRoomCard_Click;
            // 
            // Title
            // 
            Title.Anchor = AnchorStyles.Left;
            Title.AutoEllipsis = true;
            Title.AutoSize = true;
            backgroundPanel.SetColumnSpan(Title, 2);
            Title.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 204);
            Title.ForeColor = Color.Black;
            Title.Location = new Point(10, 2);
            Title.Margin = new Padding(10, 0, 0, 0);
            Title.Name = "Title";
            Title.Size = new Size(97, 21);
            Title.TabIndex = 0;
            Title.Text = "Комната \"\"";
            Title.Click += RecentRoomCard_Click;
            // 
            // lastMessageTime
            // 
            lastMessageTime.Anchor = AnchorStyles.Right;
            lastMessageTime.AutoSize = true;
            lastMessageTime.Font = new Font("Segoe UI", 8.25F);
            lastMessageTime.ForeColor = Color.Black;
            lastMessageTime.Location = new Point(303, 6);
            lastMessageTime.Margin = new Padding(0);
            lastMessageTime.Name = "lastMessageTime";
            lastMessageTime.Size = new Size(40, 13);
            lastMessageTime.TabIndex = 12;
            lastMessageTime.Text = "Время";
            lastMessageTime.Click += RecentRoomCard_Click;
            // 
            // lastMessageSenderAndContent
            // 
            lastMessageSenderAndContent.Anchor = AnchorStyles.Left;
            lastMessageSenderAndContent.AutoEllipsis = true;
            lastMessageSenderAndContent.AutoSize = true;
            backgroundPanel.SetColumnSpan(lastMessageSenderAndContent, 3);
            lastMessageSenderAndContent.Font = new Font("Segoe UI", 8.25F);
            lastMessageSenderAndContent.ForeColor = Color.Black;
            lastMessageSenderAndContent.Location = new Point(3, 31);
            lastMessageSenderAndContent.Name = "lastMessageSenderAndContent";
            lastMessageSenderAndContent.Size = new Size(158, 13);
            lastMessageSenderAndContent.TabIndex = 13;
            lastMessageSenderAndContent.Text = "Отправитель и содержимое";
            lastMessageSenderAndContent.Click += RecentRoomCard_Click;
            // 
            // participantsInfo
            // 
            participantsInfo.Anchor = AnchorStyles.Right;
            participantsInfo.AutoSize = true;
            participantsInfo.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            participantsInfo.ForeColor = Color.FromArgb(0, 0, 0);
            participantsInfo.Location = new Point(269, 5);
            participantsInfo.Margin = new Padding(0);
            participantsInfo.Name = "participantsInfo";
            participantsInfo.Size = new Size(24, 15);
            participantsInfo.TabIndex = 7;
            participantsInfo.Text = "0/0";
            participantsInfo.Click += RecentRoomCard_Click;
            // 
            // missedMessagesCountLabel
            // 
            missedMessagesCountLabel.Anchor = AnchorStyles.Right;
            missedMessagesCountLabel.AutoSize = true;
            missedMessagesCountLabel.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 204);
            missedMessagesCountLabel.ForeColor = Color.FromArgb(0, 0, 0);
            missedMessagesCountLabel.Location = new Point(321, 27);
            missedMessagesCountLabel.Name = "missedMessagesCountLabel";
            missedMessagesCountLabel.Size = new Size(19, 21);
            missedMessagesCountLabel.TabIndex = 14;
            missedMessagesCountLabel.Text = "0";
            // 
            // RecentRoomCard
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(backgroundPanel);
            Margin = new Padding(5);
            MinimumSize = new Size(0, 50);
            Name = "RecentRoomCard";
            Size = new Size(343, 50);
            Click += RecentRoomCard_Click;
            backgroundPanel.ResumeLayout(false);
            backgroundPanel.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Labels.CaptionLabel lastMessageTime;
        private Labels.Heading1Label Title;
        private Labels.CaptionLabel lastMessageSenderAndContent;
        private Labels.Heading3Label participantsInfo;
        private TableLayoutPanel backgroundPanel;
        private Labels.Heading3Label missedMessagesCountLabel;
    }
}
