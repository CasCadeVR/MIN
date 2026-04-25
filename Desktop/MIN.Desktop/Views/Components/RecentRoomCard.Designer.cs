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
            tableLayoutPanelLabels = new TableLayoutPanel();
            Title = new MIN.Desktop.Components.Labels.Heading1Label();
            lastMessageTime = new MIN.Desktop.Components.Labels.CaptionLabel();
            currentlyConnectedLabel = new MIN.Desktop.Components.Labels.CaptionLabel();
            participantsInfo = new MIN.Desktop.Components.Labels.Heading3Label();
            lastMessageSenderName = new MIN.Desktop.Components.Labels.CaptionLabel();
            lastMessageContent = new MIN.Desktop.Components.Labels.CaptionLabel();
            tableLayoutPanelLabels.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanelLabels
            // 
            tableLayoutPanelLabels.BackColor = Color.Transparent;
            tableLayoutPanelLabels.ColumnCount = 4;
            tableLayoutPanelLabels.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 87F));
            tableLayoutPanelLabels.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanelLabels.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanelLabels.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 64F));
            tableLayoutPanelLabels.Controls.Add(Title, 0, 0);
            tableLayoutPanelLabels.Controls.Add(lastMessageTime, 3, 0);
            tableLayoutPanelLabels.Controls.Add(currentlyConnectedLabel, 2, 1);
            tableLayoutPanelLabels.Controls.Add(participantsInfo, 3, 1);
            tableLayoutPanelLabels.Controls.Add(lastMessageSenderName, 0, 1);
            tableLayoutPanelLabels.Controls.Add(lastMessageContent, 1, 1);
            tableLayoutPanelLabels.Dock = DockStyle.Fill;
            tableLayoutPanelLabels.Location = new Point(0, 0);
            tableLayoutPanelLabels.Margin = new Padding(0);
            tableLayoutPanelLabels.Name = "tableLayoutPanelLabels";
            tableLayoutPanelLabels.RowCount = 2;
            tableLayoutPanelLabels.RowStyles.Add(new RowStyle(SizeType.Percent, 50.0003166F));
            tableLayoutPanelLabels.RowStyles.Add(new RowStyle(SizeType.Percent, 49.99968F));
            tableLayoutPanelLabels.Size = new Size(343, 50);
            tableLayoutPanelLabels.TabIndex = 2;
            tableLayoutPanelLabels.Click += RecentRoomCard_Click;
            // 
            // Title
            // 
            Title.AutoEllipsis = true;
            Title.AutoSize = true;
            tableLayoutPanelLabels.SetColumnSpan(Title, 2);
            Title.Dock = DockStyle.Fill;
            Title.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 204);
            Title.ForeColor = Color.Black;
            Title.Location = new Point(10, 0);
            Title.Margin = new Padding(10, 0, 0, 0);
            Title.Name = "Title";
            Title.Size = new Size(173, 25);
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
            // currentlyConnectedLabel
            // 
            currentlyConnectedLabel.Anchor = AnchorStyles.Right;
            currentlyConnectedLabel.AutoSize = true;
            currentlyConnectedLabel.Font = new Font("Segoe UI", 8.25F);
            currentlyConnectedLabel.ForeColor = Color.Black;
            currentlyConnectedLabel.Location = new Point(197, 31);
            currentlyConnectedLabel.Name = "currentlyConnectedLabel";
            currentlyConnectedLabel.Size = new Size(79, 13);
            currentlyConnectedLabel.TabIndex = 9;
            currentlyConnectedLabel.Text = "Подключено:";
            currentlyConnectedLabel.Click += RecentRoomCard_Click;
            // 
            // participantsInfo
            // 
            participantsInfo.Anchor = AnchorStyles.Left;
            participantsInfo.AutoSize = true;
            participantsInfo.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            participantsInfo.ForeColor = Color.FromArgb(0, 0, 0);
            participantsInfo.Location = new Point(282, 25);
            participantsInfo.Name = "participantsInfo";
            participantsInfo.Size = new Size(58, 25);
            participantsInfo.TabIndex = 7;
            participantsInfo.Text = "Загрузка...";
            participantsInfo.Click += RecentRoomCard_Click;
            // 
            // lastMessageSenderName
            // 
            lastMessageSenderName.Anchor = AnchorStyles.Right;
            lastMessageSenderName.AutoSize = true;
            lastMessageSenderName.Font = new Font("Segoe UI", 8.25F);
            lastMessageSenderName.ForeColor = Color.Black;
            lastMessageSenderName.Location = new Point(4, 31);
            lastMessageSenderName.Name = "lastMessageSenderName";
            lastMessageSenderName.Size = new Size(80, 13);
            lastMessageSenderName.TabIndex = 13;
            lastMessageSenderName.Text = "Отправитель:";
            lastMessageSenderName.Click += RecentRoomCard_Click;
            // 
            // lastMessageContent
            // 
            lastMessageContent.Anchor = AnchorStyles.Left;
            lastMessageContent.AutoEllipsis = true;
            lastMessageContent.AutoSize = true;
            lastMessageContent.Font = new Font("Segoe UI", 8.25F);
            lastMessageContent.ForeColor = Color.Black;
            lastMessageContent.Location = new Point(90, 31);
            lastMessageContent.Name = "lastMessageContent";
            lastMessageContent.Size = new Size(77, 13);
            lastMessageContent.TabIndex = 14;
            lastMessageContent.Text = "Содержимое";
            lastMessageContent.Click += RecentRoomCard_Click;
            // 
            // RecentRoomCard
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(tableLayoutPanelLabels);
            Margin = new Padding(5);
            MinimumSize = new Size(0, 50);
            Name = "RecentRoomCard";
            Size = new Size(343, 50);
            Click += RecentRoomCard_Click;
            tableLayoutPanelLabels.ResumeLayout(false);
            tableLayoutPanelLabels.PerformLayout();
            ResumeLayout(false);

        }

        #endregion

        private TableLayoutPanel tableLayoutPanelLabels;
        private Labels.CaptionLabel lastMessageTime;
        private Labels.Heading1Label Title;
        private Labels.CaptionLabel currentlyConnectedLabel;
        private Labels.CaptionLabel lastMessageSenderName;
        private Labels.CaptionLabel lastMessageContent;
        private Labels.Heading3Label participantsInfo;
    }
}
