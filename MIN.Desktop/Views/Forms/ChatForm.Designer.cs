namespace MIN.Desktop
{
    partial class ChatForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            splitContainerSideBar = new SplitContainer();
            splitContainer = new SplitContainer();
            tableLayoutPanelHeader = new TableLayoutPanel();
            Title = new MIN.Desktop.Components.Labels.Heading1Label();
            tableLayoutPanel2 = new TableLayoutPanel();
            chatFlow = new FlowLayoutPanel();
            tableLayoutPanelButtons = new TableLayoutPanel();
            fileButton = new MIN.Desktop.Components.CommonButton();
            sendButton = new MIN.Desktop.Components.CommonButton();
            messageTextBox = new MIN.Desktop.Components.Controls.TextBoxes.MessageTextBox();
            tableLayoutPanel1 = new TableLayoutPanel();
            disconnectButton = new MIN.Desktop.Components.InvertedButton();
            aboutButton = new MIN.Desktop.Components.CommonButton();
            participantsFlow = new FlowLayoutPanel();
            tableLayoutPanelStats = new TableLayoutPanel();
            closeButton = new MIN.Desktop.Components.InvertedButton();
            editButton = new MIN.Desktop.Components.CommonButton();
            heading3Label4 = new MIN.Desktop.Components.Labels.Heading3Label();
            captionLabel4 = new MIN.Desktop.Components.Labels.CaptionLabel();
            participantsInfo = new MIN.Desktop.Components.Labels.Heading3Label();
            captionLabel3 = new MIN.Desktop.Components.Labels.CaptionLabel();
            computer = new MIN.Desktop.Components.Labels.Heading3Label();
            captionLabel2 = new MIN.Desktop.Components.Labels.CaptionLabel();
            classroom = new MIN.Desktop.Components.Labels.Heading3Label();
            captionLabel1 = new MIN.Desktop.Components.Labels.CaptionLabel();
            hostName = new MIN.Desktop.Components.Labels.Heading3Label();
            notificationComboBox = new MIN.Desktop.Components.Controls.CheckBoxes.DefaultCheckBox();
            ((System.ComponentModel.ISupportInitialize)splitContainerSideBar).BeginInit();
            splitContainerSideBar.Panel1.SuspendLayout();
            splitContainerSideBar.Panel2.SuspendLayout();
            splitContainerSideBar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer).BeginInit();
            splitContainer.Panel1.SuspendLayout();
            splitContainer.Panel2.SuspendLayout();
            splitContainer.SuspendLayout();
            tableLayoutPanelHeader.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            tableLayoutPanelButtons.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            tableLayoutPanelStats.SuspendLayout();
            SuspendLayout();
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
            splitContainerSideBar.Panel1.Controls.Add(splitContainer);
            // 
            // splitContainerSideBar.Panel2
            // 
            splitContainerSideBar.Panel2.Controls.Add(participantsFlow);
            splitContainerSideBar.Panel2.Controls.Add(tableLayoutPanelStats);
            splitContainerSideBar.Size = new Size(504, 614);
            splitContainerSideBar.SplitterDistance = 379;
            splitContainerSideBar.SplitterWidth = 1;
            splitContainerSideBar.TabIndex = 0;
            // 
            // splitContainer
            // 
            splitContainer.Dock = DockStyle.Fill;
            splitContainer.FixedPanel = FixedPanel.Panel1;
            splitContainer.IsSplitterFixed = true;
            splitContainer.Location = new Point(0, 0);
            splitContainer.Name = "splitContainer";
            splitContainer.Orientation = Orientation.Horizontal;
            // 
            // splitContainer.Panel1
            // 
            splitContainer.Panel1.Controls.Add(tableLayoutPanelHeader);
            // 
            // splitContainer.Panel2
            // 
            splitContainer.Panel2.Controls.Add(tableLayoutPanel2);
            splitContainer.Panel2.Controls.Add(tableLayoutPanel1);
            splitContainer.Size = new Size(379, 614);
            splitContainer.SplitterDistance = 55;
            splitContainer.TabIndex = 1;
            // 
            // tableLayoutPanelHeader
            // 
            tableLayoutPanelHeader.ColumnCount = 1;
            tableLayoutPanelHeader.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanelHeader.Controls.Add(Title, 0, 0);
            tableLayoutPanelHeader.Dock = DockStyle.Fill;
            tableLayoutPanelHeader.Location = new Point(0, 0);
            tableLayoutPanelHeader.Name = "tableLayoutPanelHeader";
            tableLayoutPanelHeader.RowCount = 1;
            tableLayoutPanelHeader.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanelHeader.Size = new Size(379, 55);
            tableLayoutPanelHeader.TabIndex = 0;
            // 
            // Title
            // 
            Title.Anchor = AnchorStyles.None;
            Title.AutoSize = true;
            Title.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            Title.ForeColor = Color.FromArgb(248, 249, 255);
            Title.Location = new Point(123, 12);
            Title.Name = "Title";
            Title.Size = new Size(132, 30);
            Title.TabIndex = 0;
            Title.Text = "Комната \"\"";
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 1;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.Controls.Add(chatFlow, 0, 0);
            tableLayoutPanel2.Controls.Add(tableLayoutPanelButtons, 0, 1);
            tableLayoutPanel2.Dock = DockStyle.Fill;
            tableLayoutPanel2.Location = new Point(0, 48);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 2;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
            tableLayoutPanel2.Size = new Size(379, 507);
            tableLayoutPanel2.TabIndex = 4;
            // 
            // chatFlow
            // 
            chatFlow.AutoScroll = true;
            chatFlow.AutoScrollMargin = new Size(0, 50);
            chatFlow.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            chatFlow.Dock = DockStyle.Fill;
            chatFlow.FlowDirection = FlowDirection.BottomUp;
            chatFlow.Location = new Point(3, 3);
            chatFlow.Name = "chatFlow";
            chatFlow.Size = new Size(373, 453);
            chatFlow.TabIndex = 3;
            chatFlow.WrapContents = false;
            chatFlow.Resize += chatFlow_Resize;
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
            tableLayoutPanelButtons.Location = new Point(0, 459);
            tableLayoutPanelButtons.Margin = new Padding(0);
            tableLayoutPanelButtons.Name = "tableLayoutPanelButtons";
            tableLayoutPanelButtons.RowCount = 1;
            tableLayoutPanelButtons.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanelButtons.Size = new Size(379, 48);
            tableLayoutPanelButtons.TabIndex = 2;
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
            sendButton.Location = new Point(334, 3);
            sendButton.Margin = new Padding(3, 3, 3, 6);
            sendButton.Name = "sendButton";
            sendButton.Padding = new Padding(8, 4, 8, 4);
            sendButton.Size = new Size(42, 39);
            sendButton.TabIndex = 2;
            sendButton.UseVisualStyleBackColor = false;
            sendButton.Click += sendButton_Click;
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
            messageTextBox.Size = new Size(277, 39);
            messageTextBox.TabIndex = 4;
            messageTextBox.TextChanged += messageTextBox_TextChanged;
            messageTextBox.KeyPress += messageTextBox_KeyPress;
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
            tableLayoutPanel1.Size = new Size(379, 48);
            tableLayoutPanel1.TabIndex = 1;
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
            disconnectButton.Click += disconnectButton_Click;
            // 
            // aboutButton
            // 
            aboutButton.BackColor = Color.FromArgb(167, 157, 255);
            aboutButton.Dock = DockStyle.Right;
            aboutButton.FlatAppearance.BorderColor = Color.FromArgb(228, 230, 240);
            aboutButton.FlatStyle = FlatStyle.Flat;
            aboutButton.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            aboutButton.ForeColor = Color.FromArgb(248, 249, 255);
            aboutButton.Location = new Point(264, 3);
            aboutButton.Name = "aboutButton";
            aboutButton.Padding = new Padding(8, 4, 8, 4);
            aboutButton.Size = new Size(112, 42);
            aboutButton.TabIndex = 4;
            aboutButton.Text = "О комнате";
            aboutButton.UseVisualStyleBackColor = false;
            aboutButton.Click += aboutButton_Click;
            // 
            // participantsFlow
            // 
            participantsFlow.AutoScroll = true;
            participantsFlow.Dock = DockStyle.Fill;
            participantsFlow.FlowDirection = FlowDirection.TopDown;
            participantsFlow.Location = new Point(0, 276);
            participantsFlow.Name = "participantsFlow";
            participantsFlow.Size = new Size(124, 338);
            participantsFlow.TabIndex = 5;
            participantsFlow.WrapContents = false;
            // 
            // tableLayoutPanelStats
            // 
            tableLayoutPanelStats.ColumnCount = 2;
            tableLayoutPanelStats.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanelStats.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanelStats.Controls.Add(closeButton, 0, 0);
            tableLayoutPanelStats.Controls.Add(editButton, 1, 0);
            tableLayoutPanelStats.Controls.Add(heading3Label4, 0, 6);
            tableLayoutPanelStats.Controls.Add(captionLabel4, 0, 5);
            tableLayoutPanelStats.Controls.Add(participantsInfo, 1, 5);
            tableLayoutPanelStats.Controls.Add(captionLabel3, 0, 4);
            tableLayoutPanelStats.Controls.Add(computer, 1, 4);
            tableLayoutPanelStats.Controls.Add(captionLabel2, 0, 3);
            tableLayoutPanelStats.Controls.Add(classroom, 1, 3);
            tableLayoutPanelStats.Controls.Add(captionLabel1, 0, 2);
            tableLayoutPanelStats.Controls.Add(hostName, 1, 2);
            tableLayoutPanelStats.Controls.Add(notificationComboBox, 0, 1);
            tableLayoutPanelStats.Dock = DockStyle.Top;
            tableLayoutPanelStats.Location = new Point(0, 0);
            tableLayoutPanelStats.Margin = new Padding(0);
            tableLayoutPanelStats.Name = "tableLayoutPanelStats";
            tableLayoutPanelStats.RowCount = 7;
            tableLayoutPanelStats.RowStyles.Add(new RowStyle(SizeType.Percent, 15.7223635F));
            tableLayoutPanelStats.RowStyles.Add(new RowStyle(SizeType.Percent, 15.7223606F));
            tableLayoutPanelStats.RowStyles.Add(new RowStyle(SizeType.Percent, 15.7223606F));
            tableLayoutPanelStats.RowStyles.Add(new RowStyle(SizeType.Percent, 15.7223606F));
            tableLayoutPanelStats.RowStyles.Add(new RowStyle(SizeType.Percent, 15.7223606F));
            tableLayoutPanelStats.RowStyles.Add(new RowStyle(SizeType.Percent, 15.7251291F));
            tableLayoutPanelStats.RowStyles.Add(new RowStyle(SizeType.Percent, 5.663069F));
            tableLayoutPanelStats.Size = new Size(124, 276);
            tableLayoutPanelStats.TabIndex = 4;
            // 
            // closeButton
            // 
            closeButton.BackColor = Color.FromArgb(248, 249, 255);
            closeButton.DialogResult = DialogResult.Cancel;
            closeButton.Dock = DockStyle.Fill;
            closeButton.FlatAppearance.BorderColor = Color.FromArgb(167, 157, 255);
            closeButton.FlatAppearance.BorderSize = 2;
            closeButton.FlatStyle = FlatStyle.Flat;
            closeButton.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            closeButton.ForeColor = Color.FromArgb(167, 157, 255);
            closeButton.Location = new Point(3, 3);
            closeButton.Name = "closeButton";
            closeButton.Padding = new Padding(8, 4, 8, 4);
            closeButton.Size = new Size(56, 37);
            closeButton.TabIndex = 3;
            closeButton.Text = "X";
            closeButton.UseVisualStyleBackColor = false;
            closeButton.Click += closeButton_Click;
            // 
            // editButton
            // 
            editButton.BackColor = Color.FromArgb(167, 157, 255);
            editButton.Dock = DockStyle.Fill;
            editButton.FlatAppearance.BorderColor = Color.FromArgb(228, 230, 240);
            editButton.FlatStyle = FlatStyle.Flat;
            editButton.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            editButton.ForeColor = Color.FromArgb(248, 249, 255);
            editButton.Location = new Point(65, 3);
            editButton.Name = "editButton";
            editButton.Padding = new Padding(8, 4, 8, 4);
            editButton.Size = new Size(56, 37);
            editButton.TabIndex = 17;
            editButton.Text = ". . .";
            editButton.UseVisualStyleBackColor = false;
            editButton.Click += editButton_Click;
            // 
            // heading3Label4
            // 
            heading3Label4.Anchor = AnchorStyles.Bottom;
            heading3Label4.AutoSize = true;
            tableLayoutPanelStats.SetColumnSpan(heading3Label4, 2);
            heading3Label4.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            heading3Label4.ForeColor = Color.FromArgb(0, 0, 0);
            heading3Label4.Location = new Point(23, 259);
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
            captionLabel4.Location = new Point(17, 230);
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
            participantsInfo.Location = new Point(65, 219);
            participantsInfo.Name = "participantsInfo";
            participantsInfo.Size = new Size(55, 34);
            participantsInfo.TabIndex = 15;
            participantsInfo.Text = "Загрузка...";
            // 
            // captionLabel3
            // 
            captionLabel3.Anchor = AnchorStyles.Right;
            captionLabel3.AutoSize = true;
            captionLabel3.Font = new Font("Segoe UI", 8.25F);
            captionLabel3.ForeColor = Color.Black;
            captionLabel3.Location = new Point(15, 180);
            captionLabel3.Name = "captionLabel3";
            captionLabel3.Size = new Size(44, 26);
            captionLabel3.TabIndex = 20;
            captionLabel3.Text = "№ Компа:";
            // 
            // computer
            // 
            computer.Anchor = AnchorStyles.Left;
            computer.AutoSize = true;
            computer.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            computer.ForeColor = Color.FromArgb(0, 0, 0);
            computer.Location = new Point(65, 176);
            computer.Name = "computer";
            computer.Size = new Size(55, 34);
            computer.TabIndex = 13;
            computer.Text = "Загрузка...";
            // 
            // captionLabel2
            // 
            captionLabel2.Anchor = AnchorStyles.Right;
            captionLabel2.AutoSize = true;
            captionLabel2.Font = new Font("Segoe UI", 8.25F);
            captionLabel2.ForeColor = Color.Black;
            captionLabel2.Location = new Point(5, 144);
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
            classroom.Location = new Point(65, 133);
            classroom.Name = "classroom";
            classroom.Size = new Size(55, 34);
            classroom.TabIndex = 12;
            classroom.Text = "Загрузка...";
            // 
            // captionLabel1
            // 
            captionLabel1.Anchor = AnchorStyles.Right;
            captionLabel1.AutoSize = true;
            captionLabel1.Font = new Font("Segoe UI", 8.25F);
            captionLabel1.ForeColor = Color.Black;
            captionLabel1.Location = new Point(26, 101);
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
            hostName.Location = new Point(65, 90);
            hostName.Name = "hostName";
            hostName.Size = new Size(55, 34);
            hostName.TabIndex = 9;
            hostName.Text = "Загрузка...";
            // 
            // notificationComboBox
            // 
            notificationComboBox.Anchor = AnchorStyles.None;
            notificationComboBox.AutoSize = true;
            notificationComboBox.BackColor = Color.FromArgb(106, 91, 255);
            tableLayoutPanelStats.SetColumnSpan(notificationComboBox, 2);
            notificationComboBox.Location = new Point(12, 55);
            notificationComboBox.Name = "notificationComboBox";
            notificationComboBox.Size = new Size(100, 19);
            notificationComboBox.TabIndex = 22;
            notificationComboBox.Text = "Уведомления";
            notificationComboBox.UseVisualStyleBackColor = false;
            // 
            // ChatForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(504, 614);
            Controls.Add(splitContainerSideBar);
            Margin = new Padding(3, 4, 3, 4);
            MinimumSize = new Size(250, 298);
            Name = "ChatForm";
            Text = "MIN";
            splitContainerSideBar.Panel1.ResumeLayout(false);
            splitContainerSideBar.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerSideBar).EndInit();
            splitContainerSideBar.ResumeLayout(false);
            splitContainer.Panel1.ResumeLayout(false);
            splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer).EndInit();
            splitContainer.ResumeLayout(false);
            tableLayoutPanelHeader.ResumeLayout(false);
            tableLayoutPanelHeader.PerformLayout();
            tableLayoutPanel2.ResumeLayout(false);
            tableLayoutPanelButtons.ResumeLayout(false);
            tableLayoutPanelButtons.PerformLayout();
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanelStats.ResumeLayout(false);
            tableLayoutPanelStats.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        private SplitContainer splitContainerSideBar;
        private SplitContainer splitContainer;
        private TableLayoutPanel tableLayoutPanelHeader;
        private Components.Labels.Heading1Label Title;
        private FlowLayoutPanel chatFlow;
        private TableLayoutPanel tableLayoutPanelButtons;
        private Components.CommonButton fileButton;
        private Components.CommonButton sendButton;
        private TableLayoutPanel tableLayoutPanel1;
        private Components.InvertedButton disconnectButton;
        private Components.CommonButton aboutButton;
        private FlowLayoutPanel participantsFlow;
        private TableLayoutPanel tableLayoutPanelStats;
        private Components.Labels.Heading3Label heading3Label4;
        private Components.Labels.Heading3Label classroom;
        private Components.InvertedButton closeButton;
        private Components.Labels.Heading3Label hostName;
        private Components.Labels.Heading3Label participantsInfo;
        private Components.Labels.Heading3Label computer;
        private Components.CommonButton editButton;
        private Components.Controls.TextBoxes.MessageTextBox messageTextBox;
        private TableLayoutPanel tableLayoutPanel2;
        private Components.Labels.CaptionLabel captionLabel3;
        private Components.Labels.CaptionLabel captionLabel2;
        private Components.Labels.CaptionLabel captionLabel1;
        private Components.Labels.CaptionLabel captionLabel4;
        private NotifyIcon notifyIcon;
        private Components.Controls.CheckBoxes.DefaultCheckBox notificationComboBox;
    }
}
