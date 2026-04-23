namespace MIN.Desktop.Views.Panels.PanelViews
{
    partial class ChatPanelView
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
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
            splitContainerSideBar = new SplitContainer();
            tableLayoutPanelHeader = new TableLayoutPanel();
            Title = new MIN.Desktop.Components.Labels.Heading1Label();
            participantsFlow = new MIN.Desktop.Components.Controls.FlowLayoutPanels.NoHorizontalScrollListView();
            tableLayoutPanelStats = new TableLayoutPanel();
            createdAt = new MIN.Desktop.Components.Labels.Heading3Label();
            labelCreatedAt = new MIN.Desktop.Components.Labels.CaptionLabel();
            closeButton = new MIN.Desktop.Components.Controls.Buttons.InvertedButton();
            editButton = new MIN.Desktop.Components.Controls.Buttons.CommonButton();
            captionLabel1 = new MIN.Desktop.Components.Labels.CaptionLabel();
            hostName = new MIN.Desktop.Components.Labels.Heading3Label();
            notificationComboBox = new MIN.Desktop.Components.Controls.CheckBoxes.DefaultCheckBox();
            heading3Label4 = new MIN.Desktop.Components.Labels.Heading3Label();
            captionLabel4 = new MIN.Desktop.Components.Labels.CaptionLabel();
            participantsInfo = new MIN.Desktop.Components.Labels.Heading3Label();
            captionLabel3 = new MIN.Desktop.Components.Labels.CaptionLabel();
            computer = new MIN.Desktop.Components.Labels.Heading3Label();
            captionLabel2 = new MIN.Desktop.Components.Labels.CaptionLabel();
            classroom = new MIN.Desktop.Components.Labels.Heading3Label();
            chatFlow = new MIN.Desktop.Components.Controls.FlowLayoutPanels.NoHorizontalScrollListView();
            tableLayoutPanel1 = new TableLayoutPanel();
            disconnectButton = new MIN.Desktop.Components.Controls.Buttons.InvertedButton();
            aboutButton = new MIN.Desktop.Components.Controls.Buttons.CommonButton();
            tableLayoutPanelButtons = new TableLayoutPanel();
            fileButton = new MIN.Desktop.Components.Controls.Buttons.CommonButton();
            sendButton = new MIN.Desktop.Components.Controls.Buttons.CommonButton();
            messageTextBox = new MIN.Desktop.Components.Controls.TextBoxes.MessageTextBox();
            ((System.ComponentModel.ISupportInitialize)splitContainer).BeginInit();
            splitContainer.Panel1.SuspendLayout();
            splitContainer.Panel2.SuspendLayout();
            splitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainerSideBar).BeginInit();
            splitContainerSideBar.Panel1.SuspendLayout();
            splitContainerSideBar.Panel2.SuspendLayout();
            splitContainerSideBar.SuspendLayout();
            tableLayoutPanelHeader.SuspendLayout();
            tableLayoutPanelStats.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            tableLayoutPanelButtons.SuspendLayout();
            SuspendLayout();
            // 
            // splitContainer
            // 
            // 
            // splitContainer.Panel1
            // 
            splitContainer.Panel1.Controls.Add(tableLayoutPanelHeader);
            // 
            // splitContainer.Panel2
            // 
            splitContainer.Panel2.Controls.Add(splitContainerSideBar);
            // 
            // splitContainerSideBar
            // 
            splitContainerSideBar.Dock = DockStyle.Fill;
            splitContainerSideBar.FixedPanel = FixedPanel.Panel2;
            splitContainerSideBar.Location = new Point(0, 0);
            splitContainerSideBar.Name = "splitContainerSideBar";
            // 
            // splitContainerSideBar.Panel1
            // 
            splitContainerSideBar.Panel1.Controls.Add(chatFlow);
            splitContainerSideBar.Panel1.Controls.Add(tableLayoutPanel1);
            splitContainerSideBar.Panel1.Controls.Add(tableLayoutPanelButtons);
            // 
            // splitContainerSideBar.Panel2
            // 
            splitContainerSideBar.Panel2.Controls.Add(participantsFlow);
            splitContainerSideBar.Panel2.Controls.Add(tableLayoutPanelStats);
            splitContainerSideBar.Panel2MinSize = 100;
            splitContainerSideBar.Size = new Size(821, 642);
            splitContainerSideBar.SplitterDistance = 630;
            splitContainerSideBar.TabIndex = 1;
            // 
            // tableLayoutPanelHeader
            // 
            tableLayoutPanelHeader.BackColor = Color.FromArgb(167, 157, 255);
            tableLayoutPanelHeader.ColumnCount = 1;
            tableLayoutPanelHeader.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanelHeader.Controls.Add(Title, 0, 0);
            tableLayoutPanelHeader.Dock = DockStyle.Fill;
            tableLayoutPanelHeader.Location = new Point(0, 0);
            tableLayoutPanelHeader.Name = "tableLayoutPanelHeader";
            tableLayoutPanelHeader.RowCount = 1;
            tableLayoutPanelHeader.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanelHeader.Size = new Size(821, 48);
            tableLayoutPanelHeader.TabIndex = 0;
            // 
            // Title
            // 
            Title.Anchor = AnchorStyles.None;
            Title.AutoSize = true;
            Title.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            Title.ForeColor = Color.FromArgb(248, 249, 255);
            Title.Location = new Point(358, 9);
            Title.Name = "Title";
            Title.Size = new Size(104, 30);
            Title.TabIndex = 0;
            Title.Text = "Комната";
            // 
            // participantsFlow
            // 
            participantsFlow.AutoScroll = true;
            participantsFlow.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            participantsFlow.Dock = DockStyle.Fill;
            participantsFlow.FlowDirection = FlowDirection.TopDown;
            participantsFlow.Location = new Point(0, 276);
            participantsFlow.Name = "participantsFlow";
            participantsFlow.Size = new Size(187, 366);
            participantsFlow.TabIndex = 5;
            participantsFlow.WrapContents = false;
            // 
            // tableLayoutPanelStats
            // 
            tableLayoutPanelStats.ColumnCount = 2;
            tableLayoutPanelStats.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanelStats.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanelStats.Controls.Add(createdAt, 1, 3);
            tableLayoutPanelStats.Controls.Add(labelCreatedAt, 0, 3);
            tableLayoutPanelStats.Controls.Add(closeButton, 0, 0);
            tableLayoutPanelStats.Controls.Add(editButton, 1, 0);
            tableLayoutPanelStats.Controls.Add(captionLabel1, 0, 2);
            tableLayoutPanelStats.Controls.Add(hostName, 1, 2);
            tableLayoutPanelStats.Controls.Add(notificationComboBox, 0, 1);
            tableLayoutPanelStats.Controls.Add(heading3Label4, 0, 7);
            tableLayoutPanelStats.Controls.Add(captionLabel4, 0, 6);
            tableLayoutPanelStats.Controls.Add(participantsInfo, 1, 6);
            tableLayoutPanelStats.Controls.Add(captionLabel3, 0, 5);
            tableLayoutPanelStats.Controls.Add(computer, 1, 5);
            tableLayoutPanelStats.Controls.Add(captionLabel2, 0, 4);
            tableLayoutPanelStats.Controls.Add(classroom, 1, 4);
            tableLayoutPanelStats.Dock = DockStyle.Top;
            tableLayoutPanelStats.Location = new Point(0, 0);
            tableLayoutPanelStats.Margin = new Padding(0);
            tableLayoutPanelStats.Name = "tableLayoutPanelStats";
            tableLayoutPanelStats.RowCount = 8;
            tableLayoutPanelStats.RowStyles.Add(new RowStyle(SizeType.Percent, 13.58574F));
            tableLayoutPanelStats.RowStyles.Add(new RowStyle(SizeType.Percent, 13.585741F));
            tableLayoutPanelStats.RowStyles.Add(new RowStyle(SizeType.Percent, 13.585741F));
            tableLayoutPanelStats.RowStyles.Add(new RowStyle(SizeType.Percent, 13.585741F));
            tableLayoutPanelStats.RowStyles.Add(new RowStyle(SizeType.Percent, 13.585741F));
            tableLayoutPanelStats.RowStyles.Add(new RowStyle(SizeType.Percent, 13.5881348F));
            tableLayoutPanelStats.RowStyles.Add(new RowStyle(SizeType.Percent, 13.5923424F));
            tableLayoutPanelStats.RowStyles.Add(new RowStyle(SizeType.Percent, 4.890824F));
            tableLayoutPanelStats.Size = new Size(187, 276);
            tableLayoutPanelStats.TabIndex = 4;
            // 
            // createdAt
            // 
            createdAt.Anchor = AnchorStyles.Left;
            createdAt.AutoSize = true;
            createdAt.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            createdAt.ForeColor = Color.FromArgb(0, 0, 0);
            createdAt.Location = new Point(96, 121);
            createdAt.Name = "createdAt";
            createdAt.Size = new Size(74, 17);
            createdAt.TabIndex = 24;
            createdAt.Text = "Загрузка...";
            // 
            // labelCreatedAt
            // 
            labelCreatedAt.Anchor = AnchorStyles.Right;
            labelCreatedAt.AutoSize = true;
            labelCreatedAt.Font = new Font("Segoe UI", 8.25F);
            labelCreatedAt.ForeColor = Color.Black;
            labelCreatedAt.Location = new Point(36, 123);
            labelCreatedAt.Name = "labelCreatedAt";
            labelCreatedAt.Size = new Size(54, 13);
            labelCreatedAt.TabIndex = 23;
            labelCreatedAt.Text = "Создана:";
            // 
            // closeButton
            // 
            closeButton.BackColor = Color.FromArgb(248, 249, 255);
            closeButton.DialogResult = DialogResult.Cancel;
            closeButton.Dock = DockStyle.Left;
            closeButton.FlatAppearance.BorderColor = Color.FromArgb(167, 157, 255);
            closeButton.FlatAppearance.BorderSize = 2;
            closeButton.FlatStyle = FlatStyle.Flat;
            closeButton.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            closeButton.ForeColor = Color.FromArgb(167, 157, 255);
            closeButton.Location = new Point(3, 3);
            closeButton.Name = "closeButton";
            closeButton.Size = new Size(37, 31);
            closeButton.TabIndex = 3;
            closeButton.Text = "X";
            closeButton.UseVisualStyleBackColor = false;
            // 
            // editButton
            // 
            editButton.BackColor = Color.FromArgb(167, 157, 255);
            editButton.Dock = DockStyle.Right;
            editButton.FlatAppearance.BorderColor = Color.FromArgb(228, 230, 240);
            editButton.FlatStyle = FlatStyle.Flat;
            editButton.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 204);
            editButton.ForeColor = Color.FromArgb(248, 249, 255);
            editButton.Location = new Point(127, 3);
            editButton.Name = "editButton";
            editButton.Size = new Size(57, 31);
            editButton.TabIndex = 17;
            editButton.Text = ". . .";
            editButton.UseVisualStyleBackColor = false;
            // 
            // captionLabel1
            // 
            captionLabel1.Anchor = AnchorStyles.Right;
            captionLabel1.AutoSize = true;
            captionLabel1.Font = new Font("Segoe UI", 8.25F);
            captionLabel1.ForeColor = Color.Black;
            captionLabel1.Location = new Point(57, 86);
            captionLabel1.Name = "captionLabel1";
            captionLabel1.Size = new Size(33, 13);
            captionLabel1.TabIndex = 18;
            captionLabel1.Text = "Хост:";
            // 
            // hostName
            // 
            hostName.Anchor = AnchorStyles.Left;
            hostName.AutoSize = true;
            hostName.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            hostName.ForeColor = Color.FromArgb(0, 0, 0);
            hostName.Location = new Point(96, 84);
            hostName.Name = "hostName";
            hostName.Size = new Size(74, 17);
            hostName.TabIndex = 9;
            hostName.Text = "Загрузка...";
            // 
            // notificationComboBox
            // 
            notificationComboBox.Anchor = AnchorStyles.None;
            notificationComboBox.AutoSize = true;
            notificationComboBox.BackColor = Color.White;
            tableLayoutPanelStats.SetColumnSpan(notificationComboBox, 2);
            notificationComboBox.Location = new Point(43, 46);
            notificationComboBox.Name = "notificationComboBox";
            notificationComboBox.Size = new Size(100, 19);
            notificationComboBox.TabIndex = 22;
            notificationComboBox.Text = "Уведомления";
            notificationComboBox.UseVisualStyleBackColor = false;
            // 
            // heading3Label4
            // 
            heading3Label4.Anchor = AnchorStyles.Bottom;
            heading3Label4.AutoSize = true;
            tableLayoutPanelStats.SetColumnSpan(heading3Label4, 2);
            heading3Label4.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            heading3Label4.ForeColor = Color.FromArgb(0, 0, 0);
            heading3Label4.Location = new Point(54, 259);
            heading3Label4.Name = "heading3Label4";
            heading3Label4.Size = new Size(78, 17);
            heading3Label4.TabIndex = 16;
            heading3Label4.Text = "Участники:";
            // 
            // captionLabel4
            // 
            captionLabel4.Anchor = AnchorStyles.Right;
            captionLabel4.AutoSize = true;
            captionLabel4.Font = new Font("Segoe UI", 8.25F);
            captionLabel4.ForeColor = Color.Black;
            captionLabel4.Location = new Point(48, 234);
            captionLabel4.Name = "captionLabel4";
            captionLabel4.Size = new Size(42, 13);
            captionLabel4.TabIndex = 21;
            captionLabel4.Text = "В сети:";
            // 
            // participantsInfo
            // 
            participantsInfo.Anchor = AnchorStyles.Left;
            participantsInfo.AutoSize = true;
            participantsInfo.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            participantsInfo.ForeColor = Color.FromArgb(0, 0, 0);
            participantsInfo.Location = new Point(96, 232);
            participantsInfo.Name = "participantsInfo";
            participantsInfo.Size = new Size(74, 17);
            participantsInfo.TabIndex = 15;
            participantsInfo.Text = "Загрузка...";
            // 
            // captionLabel3
            // 
            captionLabel3.Anchor = AnchorStyles.Right;
            captionLabel3.AutoSize = true;
            captionLabel3.Font = new Font("Segoe UI", 8.25F);
            captionLabel3.ForeColor = Color.Black;
            captionLabel3.Location = new Point(31, 197);
            captionLabel3.Name = "captionLabel3";
            captionLabel3.Size = new Size(59, 13);
            captionLabel3.TabIndex = 20;
            captionLabel3.Text = "№ Компа:";
            // 
            // computer
            // 
            computer.Anchor = AnchorStyles.Left;
            computer.AutoSize = true;
            computer.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            computer.ForeColor = Color.FromArgb(0, 0, 0);
            computer.Location = new Point(96, 195);
            computer.Name = "computer";
            computer.Size = new Size(74, 17);
            computer.TabIndex = 13;
            computer.Text = "Загрузка...";
            // 
            // captionLabel2
            // 
            captionLabel2.Anchor = AnchorStyles.Right;
            captionLabel2.AutoSize = true;
            captionLabel2.Font = new Font("Segoe UI", 8.25F);
            captionLabel2.ForeColor = Color.Black;
            captionLabel2.Location = new Point(36, 160);
            captionLabel2.Name = "captionLabel2";
            captionLabel2.Size = new Size(54, 13);
            captionLabel2.TabIndex = 19;
            captionLabel2.Text = "Кабинет:";
            // 
            // classroom
            // 
            classroom.Anchor = AnchorStyles.Left;
            classroom.AutoSize = true;
            classroom.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            classroom.ForeColor = Color.FromArgb(0, 0, 0);
            classroom.Location = new Point(96, 158);
            classroom.Name = "classroom";
            classroom.Size = new Size(74, 17);
            classroom.TabIndex = 12;
            classroom.Text = "Загрузка...";
            // 
            // chatFlow
            // 
            chatFlow.AutoScroll = true;
            chatFlow.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            chatFlow.Dock = DockStyle.Fill;
            chatFlow.FlowDirection = FlowDirection.BottomUp;
            chatFlow.Location = new Point(0, 48);
            chatFlow.Name = "chatFlow";
            chatFlow.Size = new Size(630, 546);
            chatFlow.TabIndex = 6;
            chatFlow.WrapContents = false;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Controls.Add(disconnectButton, 0, 0);
            tableLayoutPanel1.Controls.Add(aboutButton, 1, 0);
            tableLayoutPanel1.Dock = DockStyle.Top;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Margin = new Padding(0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 1;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3333321F));
            tableLayoutPanel1.Size = new Size(630, 48);
            tableLayoutPanel1.TabIndex = 4;
            // 
            // disconnectButton
            // 
            disconnectButton.BackColor = Color.FromArgb(248, 249, 255);
            disconnectButton.DialogResult = DialogResult.Cancel;
            disconnectButton.Dock = DockStyle.Left;
            disconnectButton.FlatAppearance.BorderColor = Color.FromArgb(167, 157, 255);
            disconnectButton.FlatAppearance.BorderSize = 2;
            disconnectButton.FlatStyle = FlatStyle.Flat;
            disconnectButton.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            disconnectButton.ForeColor = Color.FromArgb(167, 157, 255);
            disconnectButton.Location = new Point(3, 3);
            disconnectButton.Name = "disconnectButton";
            disconnectButton.Padding = new Padding(8, 4, 8, 4);
            disconnectButton.Size = new Size(104, 42);
            disconnectButton.TabIndex = 3;
            disconnectButton.Text = "Выйти";
            disconnectButton.UseVisualStyleBackColor = false;
            // 
            // aboutButton
            // 
            aboutButton.BackColor = Color.FromArgb(167, 157, 255);
            aboutButton.Dock = DockStyle.Right;
            aboutButton.FlatAppearance.BorderColor = Color.FromArgb(228, 230, 240);
            aboutButton.FlatStyle = FlatStyle.Flat;
            aboutButton.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            aboutButton.ForeColor = Color.FromArgb(248, 249, 255);
            aboutButton.Location = new Point(515, 3);
            aboutButton.Name = "aboutButton";
            aboutButton.Padding = new Padding(8, 4, 8, 4);
            aboutButton.Size = new Size(112, 42);
            aboutButton.TabIndex = 4;
            aboutButton.Text = "О комнате";
            aboutButton.UseVisualStyleBackColor = false;
            // 
            // tableLayoutPanelButtons
            // 
            tableLayoutPanelButtons.ColumnCount = 3;
            tableLayoutPanelButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 48F));
            tableLayoutPanelButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanelButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 48F));
            tableLayoutPanelButtons.Controls.Add(fileButton, 0, 0);
            tableLayoutPanelButtons.Controls.Add(sendButton, 2, 0);
            tableLayoutPanelButtons.Controls.Add(messageTextBox, 1, 0);
            tableLayoutPanelButtons.Dock = DockStyle.Bottom;
            tableLayoutPanelButtons.Location = new Point(0, 594);
            tableLayoutPanelButtons.Margin = new Padding(0);
            tableLayoutPanelButtons.Name = "tableLayoutPanelButtons";
            tableLayoutPanelButtons.RowCount = 1;
            tableLayoutPanelButtons.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanelButtons.Size = new Size(630, 48);
            tableLayoutPanelButtons.TabIndex = 5;
            // 
            // fileButton
            // 
            fileButton.BackColor = Color.FromArgb(192, 192, 255);
            fileButton.BackgroundImage = Properties.Resources.paperclip;
            fileButton.BackgroundImageLayout = ImageLayout.Zoom;
            fileButton.Dock = DockStyle.Top;
            fileButton.FlatAppearance.BorderColor = Color.FromArgb(228, 230, 240);
            fileButton.FlatStyle = FlatStyle.Flat;
            fileButton.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 204);
            fileButton.ForeColor = Color.FromArgb(248, 249, 255);
            fileButton.Location = new Point(3, 3);
            fileButton.Margin = new Padding(3, 3, 3, 6);
            fileButton.Name = "fileButton";
            fileButton.Padding = new Padding(8, 4, 8, 4);
            fileButton.Size = new Size(42, 39);
            fileButton.TabIndex = 3;
            fileButton.UseVisualStyleBackColor = false;
            // 
            // sendButton
            // 
            sendButton.BackColor = Color.FromArgb(192, 192, 255);
            sendButton.BackgroundImage = Properties.Resources.send;
            sendButton.BackgroundImageLayout = ImageLayout.Zoom;
            sendButton.Dock = DockStyle.Top;
            sendButton.FlatAppearance.BorderColor = Color.FromArgb(228, 230, 240);
            sendButton.FlatStyle = FlatStyle.Flat;
            sendButton.Font = new Font("Segoe UI Black", 9F, FontStyle.Bold, GraphicsUnit.Point, 204);
            sendButton.ForeColor = Color.FromArgb(248, 249, 255);
            sendButton.Location = new Point(585, 3);
            sendButton.Margin = new Padding(3, 3, 3, 6);
            sendButton.Name = "sendButton";
            sendButton.Padding = new Padding(8, 4, 8, 4);
            sendButton.Size = new Size(42, 39);
            sendButton.TabIndex = 2;
            sendButton.UseVisualStyleBackColor = false;
            // 
            // messageTextBox
            // 
            messageTextBox.AcceptsReturn = true;
            messageTextBox.BackColor = Color.FromArgb(248, 249, 255);
            messageTextBox.BorderStyle = BorderStyle.None;
            messageTextBox.Dock = DockStyle.Fill;
            messageTextBox.Font = new Font("Segoe UI", 9.75F);
            messageTextBox.ForeColor = Color.FromArgb(122, 119, 143);
            messageTextBox.Location = new Point(51, 3);
            messageTextBox.Margin = new Padding(3, 3, 3, 6);
            messageTextBox.MaxLength = 65526;
            messageTextBox.Multiline = true;
            messageTextBox.Name = "messageTextBox";
            messageTextBox.PlaceholderText = "Сообщение";
            messageTextBox.ScrollBars = ScrollBars.Vertical;
            messageTextBox.Size = new Size(528, 39);
            messageTextBox.TabIndex = 4;
            // 
            // ChatPanelView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Name = "ChatPanelView";
            Controls.SetChildIndex(splitContainer, 0);
            splitContainer.Panel1.ResumeLayout(false);
            splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer).EndInit();
            splitContainer.ResumeLayout(false);
            splitContainerSideBar.Panel1.ResumeLayout(false);
            splitContainerSideBar.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerSideBar).EndInit();
            splitContainerSideBar.ResumeLayout(false);
            tableLayoutPanelHeader.ResumeLayout(false);
            tableLayoutPanelHeader.PerformLayout();
            tableLayoutPanelStats.ResumeLayout(false);
            tableLayoutPanelStats.PerformLayout();
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanelButtons.ResumeLayout(false);
            tableLayoutPanelButtons.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private SplitContainer splitContainerSideBar;
        private TableLayoutPanel tableLayoutPanelHeader;
        private Desktop.Components.Labels.Heading1Label Title;
        private Desktop.Components.Controls.FlowLayoutPanels.NoHorizontalScrollListView participantsFlow;
        private TableLayoutPanel tableLayoutPanelStats;
        private Desktop.Components.Labels.Heading3Label createdAt;
        private Desktop.Components.Labels.CaptionLabel labelCreatedAt;
        private Desktop.Components.Controls.Buttons.InvertedButton closeButton;
        private Desktop.Components.Controls.Buttons.CommonButton editButton;
        private Desktop.Components.Labels.CaptionLabel captionLabel1;
        private Desktop.Components.Labels.Heading3Label hostName;
        private Desktop.Components.Controls.CheckBoxes.DefaultCheckBox notificationComboBox;
        private Desktop.Components.Labels.Heading3Label heading3Label4;
        private Desktop.Components.Labels.CaptionLabel captionLabel4;
        private Desktop.Components.Labels.Heading3Label participantsInfo;
        private Desktop.Components.Labels.CaptionLabel captionLabel3;
        private Desktop.Components.Labels.Heading3Label computer;
        private Desktop.Components.Labels.CaptionLabel captionLabel2;
        private Desktop.Components.Labels.Heading3Label classroom;
        private Desktop.Components.Controls.FlowLayoutPanels.NoHorizontalScrollListView chatFlow;
        private TableLayoutPanel tableLayoutPanel1;
        private Desktop.Components.Controls.Buttons.InvertedButton disconnectButton;
        private Desktop.Components.Controls.Buttons.CommonButton aboutButton;
        private TableLayoutPanel tableLayoutPanelButtons;
        private Desktop.Components.Controls.Buttons.CommonButton fileButton;
        private Desktop.Components.Controls.Buttons.CommonButton sendButton;
        private Desktop.Components.Controls.TextBoxes.MessageTextBox messageTextBox;
    }
}
