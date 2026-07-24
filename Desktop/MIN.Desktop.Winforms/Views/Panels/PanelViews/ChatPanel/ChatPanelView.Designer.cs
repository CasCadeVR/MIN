using MIN.Desktop.Winforms.Properties;

namespace MIN.Desktop.Views.Panels.PanelViews.ChatPanel
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
            chatFlow = new MIN.Desktop.Components.Controls.FlowLayoutPanels.NoHorizontalScrollListView();
            tableLayoutPanelButtons = new TableLayoutPanel();
            actionButton = new MIN.Desktop.Components.Controls.Buttons.CommonButton();
            sendButton = new MIN.Desktop.Components.Controls.Buttons.CommonButton();
            messageTextBox = new MIN.Desktop.Components.Controls.TextBoxes.MessageTextBox();
            multiFileAttachmentUploader = new MIN.Desktop.Components.ComplexControls.MultiFileAttachmentUploader();
            statusLabel = new MIN.Desktop.Components.Labels.CaptionLabel();
            participantsFlow = new MIN.Desktop.Components.Controls.FlowLayoutPanels.NoHorizontalScrollListView();
            tableLayoutPanelStats = new TableLayoutPanel();
            connectionPort = new MIN.Desktop.Components.Textboxes.ReadonlyTextbox();
            connectionPortLabel = new MIN.Desktop.Components.Labels.CaptionLabel();
            connectionAddressLabel = new MIN.Desktop.Components.Labels.CaptionLabel();
            createdAt = new MIN.Desktop.Components.Labels.Heading3Label();
            createdAtLabel = new MIN.Desktop.Components.Labels.CaptionLabel();
            closeButton = new MIN.Desktop.Components.Controls.Buttons.InvertedButton();
            editButton = new MIN.Desktop.Components.Controls.Buttons.CommonButton();
            hostNameLabel = new MIN.Desktop.Components.Labels.CaptionLabel();
            hostName = new MIN.Desktop.Components.Labels.Heading3Label();
            notificationComboBox = new MIN.Desktop.Components.Controls.CheckBoxes.DefaultCheckBox();
            participantsLabel = new MIN.Desktop.Components.Labels.Heading3Label();
            computerLabel = new MIN.Desktop.Components.Labels.CaptionLabel();
            computer = new MIN.Desktop.Components.Labels.Heading3Label();
            classroomLabel = new MIN.Desktop.Components.Labels.CaptionLabel();
            classroom = new MIN.Desktop.Components.Labels.Heading3Label();
            onlineLabel = new MIN.Desktop.Components.Labels.CaptionLabel();
            participantsInfo = new MIN.Desktop.Components.Labels.Heading3Label();
            connectionAddress = new MIN.Desktop.Components.Textboxes.ReadonlyTextbox();
            disconnectButton = new MIN.Desktop.Components.Controls.Buttons.InvertedButton();
            aboutButton = new MIN.Desktop.Components.Controls.Buttons.CommonButton();
            tableLayoutPanelHeader = new TableLayoutPanel();
            Title = new MIN.Desktop.Components.Labels.Heading1Label();
            openFileDialog = new OpenFileDialog();
            chatActionContextMenuStrip = new MIN.Desktop.Components.Controls.ContextMenuStrips.ChatActionContextMenuStrip();
            ((System.ComponentModel.ISupportInitialize)splitContainer).BeginInit();
            splitContainer.Panel1.SuspendLayout();
            splitContainer.Panel2.SuspendLayout();
            splitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainerSideBar).BeginInit();
            splitContainerSideBar.Panel1.SuspendLayout();
            splitContainerSideBar.Panel2.SuspendLayout();
            splitContainerSideBar.SuspendLayout();
            tableLayoutPanelButtons.SuspendLayout();
            tableLayoutPanelStats.SuspendLayout();
            tableLayoutPanelHeader.SuspendLayout();
            SuspendLayout();
            // 
            // splitContainer
            // 
            splitContainer.BackColor = Color.Transparent;
            splitContainer.ForeColor = Color.FromArgb(45, 43, 58);
            // 
            // splitContainer.Panel1
            // 
            splitContainer.Panel1.Controls.Add(tableLayoutPanelHeader);
            // 
            // splitContainer.Panel2
            // 
            splitContainer.Panel2.Controls.Add(splitContainerSideBar);
            splitContainer.SplitterWidth = 1;
            // 
            // splitContainerSideBar
            // 
            splitContainerSideBar.Dock = DockStyle.Fill;
            splitContainerSideBar.FixedPanel = FixedPanel.Panel2;
            splitContainerSideBar.IsSplitterFixed = true;
            splitContainerSideBar.Location = new Point(0, 0);
            splitContainerSideBar.Name = "splitContainerSideBar";
            // 
            // splitContainerSideBar.Panel1
            // 
            splitContainerSideBar.Panel1.Controls.Add(chatFlow);
            splitContainerSideBar.Panel1.Controls.Add(tableLayoutPanelButtons);
            // 
            // splitContainerSideBar.Panel2
            // 
            splitContainerSideBar.Panel2.Controls.Add(participantsFlow);
            splitContainerSideBar.Panel2.Controls.Add(tableLayoutPanelStats);
            splitContainerSideBar.Panel2MinSize = 100;
            splitContainerSideBar.Size = new Size(821, 643);
            splitContainerSideBar.SplitterDistance = 633;
            splitContainerSideBar.SplitterWidth = 1;
            splitContainerSideBar.TabIndex = 1;
            // 
            // chatFlow
            // 
            chatFlow.AllowDrop = true;
            chatFlow.AutoScroll = true;
            chatFlow.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            chatFlow.Dock = DockStyle.Fill;
            chatFlow.FlowDirection = FlowDirection.BottomUp;
            chatFlow.Location = new Point(0, 0);
            chatFlow.Name = "chatFlow";
            chatFlow.Size = new Size(633, 468);
            chatFlow.TabIndex = 6;
            chatFlow.WrapContents = false;
            chatFlow.DragDrop += chatFlow_DragDrop;
            chatFlow.DragEnter += chatFlow_DragEnter;
            chatFlow.DragOver += chatFlow_DragOver;
            chatFlow.DragLeave += chatFlow_DragLeave;
            chatFlow.Resize += chatFlow_Resize;
            // 
            // tableLayoutPanelButtons
            // 
            tableLayoutPanelButtons.ColumnCount = 3;
            tableLayoutPanelButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 45F));
            tableLayoutPanelButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanelButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 48F));
            tableLayoutPanelButtons.Controls.Add(actionButton, 0, 2);
            tableLayoutPanelButtons.Controls.Add(sendButton, 2, 2);
            tableLayoutPanelButtons.Controls.Add(messageTextBox, 1, 2);
            tableLayoutPanelButtons.Controls.Add(multiFileAttachmentUploader, 0, 1);
            tableLayoutPanelButtons.Controls.Add(statusLabel, 0, 0);
            tableLayoutPanelButtons.Dock = DockStyle.Bottom;
            tableLayoutPanelButtons.GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
            tableLayoutPanelButtons.Location = new Point(0, 468);
            tableLayoutPanelButtons.Margin = new Padding(0);
            tableLayoutPanelButtons.Name = "tableLayoutPanelButtons";
            tableLayoutPanelButtons.RowCount = 3;
            tableLayoutPanelButtons.RowStyles.Add(new RowStyle(SizeType.Absolute, 16F));
            tableLayoutPanelButtons.RowStyles.Add(new RowStyle(SizeType.Absolute, 115F));
            tableLayoutPanelButtons.RowStyles.Add(new RowStyle(SizeType.Absolute, 8F));
            tableLayoutPanelButtons.Size = new Size(633, 175);
            tableLayoutPanelButtons.TabIndex = 5;
            // 
            // actionButton
            // 
            actionButton.BackColor = Color.FromArgb(192, 192, 255);
            actionButton.BackgroundImage = Resources.plus;
            actionButton.BackgroundImageLayout = ImageLayout.Zoom;
            actionButton.Dock = DockStyle.Top;
            actionButton.FlatAppearance.BorderColor = Color.FromArgb(228, 230, 240);
            actionButton.FlatStyle = FlatStyle.Flat;
            actionButton.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 204);
            actionButton.ForeColor = Color.FromArgb(248, 249, 255);
            actionButton.Location = new Point(3, 134);
            actionButton.Name = "actionButton";
            actionButton.Padding = new Padding(8, 4, 8, 4);
            actionButton.Size = new Size(39, 38);
            actionButton.TabIndex = 3;
            actionButton.UseVisualStyleBackColor = false;
            actionButton.Click += actionButton_Click;
            // 
            // sendButton
            // 
            sendButton.BackColor = Color.FromArgb(192, 192, 255);
            sendButton.BackgroundImage = Resources.send;
            sendButton.BackgroundImageLayout = ImageLayout.Zoom;
            sendButton.Dock = DockStyle.Top;
            sendButton.FlatAppearance.BorderColor = Color.FromArgb(228, 230, 240);
            sendButton.FlatStyle = FlatStyle.Flat;
            sendButton.Font = new Font("Segoe UI Black", 9F, FontStyle.Bold, GraphicsUnit.Point, 204);
            sendButton.ForeColor = Color.FromArgb(248, 249, 255);
            sendButton.Location = new Point(588, 134);
            sendButton.Name = "sendButton";
            sendButton.Padding = new Padding(8, 4, 8, 4);
            sendButton.Size = new Size(42, 38);
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
            messageTextBox.Location = new Point(48, 134);
            messageTextBox.Margin = new Padding(3, 3, 3, 6);
            messageTextBox.MaxLength = 65526;
            messageTextBox.Multiline = true;
            messageTextBox.Name = "messageTextBox";
            messageTextBox.PlaceholderText = "Сообщение";
            messageTextBox.ScrollBars = ScrollBars.Vertical;
            messageTextBox.Size = new Size(534, 35);
            messageTextBox.TabIndex = 4;
            messageTextBox.TextChanged += messageTextBox_TextChanged;
            messageTextBox.KeyDown += messageTextBox_KeyDown;
            messageTextBox.KeyPress += messageTextBox_KeyPress;
            // 
            // multiFileAttachmentUploader
            // 
            multiFileAttachmentUploader.AutoScroll = true;
            multiFileAttachmentUploader.BackColor = SystemColors.Control;
            tableLayoutPanelButtons.SetColumnSpan(multiFileAttachmentUploader, 3);
            multiFileAttachmentUploader.Dock = DockStyle.Fill;
            multiFileAttachmentUploader.Location = new Point(0, 16);
            multiFileAttachmentUploader.Margin = new Padding(0);
            multiFileAttachmentUploader.Name = "multiFileAttachmentUploader";
            multiFileAttachmentUploader.OnLastFileRemoved = null;
            multiFileAttachmentUploader.Padding = new Padding(5);
            multiFileAttachmentUploader.Size = new Size(633, 115);
            multiFileAttachmentUploader.TabIndex = 5;
            // 
            // statusLabel
            // 
            statusLabel.AutoEllipsis = true;
            statusLabel.AutoSize = true;
            tableLayoutPanelButtons.SetColumnSpan(statusLabel, 3);
            statusLabel.Dock = DockStyle.Fill;
            statusLabel.Font = new Font("Segoe UI", 8F);
            statusLabel.ForeColor = Color.Black;
            statusLabel.Location = new Point(3, 0);
            statusLabel.Name = "statusLabel";
            statusLabel.Size = new Size(627, 16);
            statusLabel.TabIndex = 6;
            statusLabel.Text = "statusLabel";
            statusLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // participantsFlow
            // 
            participantsFlow.AutoScroll = true;
            participantsFlow.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            participantsFlow.Dock = DockStyle.Fill;
            participantsFlow.FlowDirection = FlowDirection.TopDown;
            participantsFlow.Location = new Point(0, 270);
            participantsFlow.Name = "participantsFlow";
            participantsFlow.Size = new Size(187, 373);
            participantsFlow.TabIndex = 5;
            participantsFlow.WrapContents = false;
            participantsFlow.Resize += participantsFlow_Resize;
            // 
            // tableLayoutPanelStats
            // 
            tableLayoutPanelStats.ColumnCount = 2;
            tableLayoutPanelStats.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanelStats.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanelStats.Controls.Add(connectionPort, 1, 7);
            tableLayoutPanelStats.Controls.Add(connectionPortLabel, 0, 7);
            tableLayoutPanelStats.Controls.Add(connectionAddressLabel, 0, 6);
            tableLayoutPanelStats.Controls.Add(createdAt, 1, 3);
            tableLayoutPanelStats.Controls.Add(createdAtLabel, 0, 3);
            tableLayoutPanelStats.Controls.Add(closeButton, 0, 0);
            tableLayoutPanelStats.Controls.Add(editButton, 1, 0);
            tableLayoutPanelStats.Controls.Add(hostNameLabel, 0, 2);
            tableLayoutPanelStats.Controls.Add(hostName, 1, 2);
            tableLayoutPanelStats.Controls.Add(notificationComboBox, 0, 1);
            tableLayoutPanelStats.Controls.Add(participantsLabel, 0, 9);
            tableLayoutPanelStats.Controls.Add(computerLabel, 0, 5);
            tableLayoutPanelStats.Controls.Add(computer, 1, 5);
            tableLayoutPanelStats.Controls.Add(classroomLabel, 0, 4);
            tableLayoutPanelStats.Controls.Add(classroom, 1, 4);
            tableLayoutPanelStats.Controls.Add(onlineLabel, 0, 8);
            tableLayoutPanelStats.Controls.Add(participantsInfo, 1, 8);
            tableLayoutPanelStats.Controls.Add(connectionAddress, 1, 6);
            tableLayoutPanelStats.Dock = DockStyle.Top;
            tableLayoutPanelStats.Location = new Point(0, 0);
            tableLayoutPanelStats.Margin = new Padding(0);
            tableLayoutPanelStats.Name = "tableLayoutPanelStats";
            tableLayoutPanelStats.RowCount = 10;
            tableLayoutPanelStats.RowStyles.Add(new RowStyle(SizeType.Absolute, 37F));
            tableLayoutPanelStats.RowStyles.Add(new RowStyle(SizeType.Percent, 11.9591761F));
            tableLayoutPanelStats.RowStyles.Add(new RowStyle(SizeType.Percent, 11.9591761F));
            tableLayoutPanelStats.RowStyles.Add(new RowStyle(SizeType.Percent, 11.9591761F));
            tableLayoutPanelStats.RowStyles.Add(new RowStyle(SizeType.Percent, 11.9591761F));
            tableLayoutPanelStats.RowStyles.Add(new RowStyle(SizeType.Percent, 11.9612818F));
            tableLayoutPanelStats.RowStyles.Add(new RowStyle(SizeType.Percent, 11.962925F));
            tableLayoutPanelStats.RowStyles.Add(new RowStyle(SizeType.Percent, 11.9688425F));
            tableLayoutPanelStats.RowStyles.Add(new RowStyle(SizeType.Percent, 11.9649839F));
            tableLayoutPanelStats.RowStyles.Add(new RowStyle(SizeType.Percent, 4.30526543F));
            tableLayoutPanelStats.Size = new Size(187, 270);
            tableLayoutPanelStats.TabIndex = 4;
            // 
            // connectionPort
            // 
            connectionPort.Anchor = AnchorStyles.Left;
            connectionPort.BackColor = Color.FromArgb(248, 249, 255);
            connectionPort.BorderStyle = BorderStyle.None;
            connectionPort.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 204);
            connectionPort.ForeColor = Color.Black;
            connectionPort.Location = new Point(96, 203);
            connectionPort.Name = "connectionPort";
            connectionPort.ReadOnly = true;
            connectionPort.Size = new Size(88, 18);
            connectionPort.TabIndex = 30;
            connectionPort.Text = "Загрузка...";
            // 
            // connectionPortLabel
            // 
            connectionPortLabel.Anchor = AnchorStyles.Right;
            connectionPortLabel.AutoSize = true;
            connectionPortLabel.Font = new Font("Segoe UI", 8.25F);
            connectionPortLabel.ForeColor = Color.Black;
            connectionPortLabel.Location = new Point(53, 206);
            connectionPortLabel.Name = "connectionPortLabel";
            connectionPortLabel.Size = new Size(37, 13);
            connectionPortLabel.TabIndex = 27;
            connectionPortLabel.Text = "Порт:";
            // 
            // connectionAddressLabel
            // 
            connectionAddressLabel.Anchor = AnchorStyles.Right;
            connectionAddressLabel.AutoSize = true;
            connectionAddressLabel.Font = new Font("Segoe UI", 8.25F);
            connectionAddressLabel.ForeColor = Color.Black;
            connectionAddressLabel.Location = new Point(49, 179);
            connectionAddressLabel.Name = "connectionAddressLabel";
            connectionAddressLabel.Size = new Size(41, 13);
            connectionAddressLabel.TabIndex = 25;
            connectionAddressLabel.Text = "Адрес:";
            // 
            // createdAt
            // 
            createdAt.Anchor = AnchorStyles.Left;
            createdAt.AutoSize = true;
            createdAt.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            createdAt.ForeColor = Color.FromArgb(0, 0, 0);
            createdAt.Location = new Point(96, 96);
            createdAt.Name = "createdAt";
            createdAt.Size = new Size(74, 17);
            createdAt.TabIndex = 24;
            createdAt.Text = "Загрузка...";
            // 
            // createdAtLabel
            // 
            createdAtLabel.Anchor = AnchorStyles.Right;
            createdAtLabel.AutoSize = true;
            createdAtLabel.Font = new Font("Segoe UI", 8.25F);
            createdAtLabel.ForeColor = Color.Black;
            createdAtLabel.Location = new Point(36, 98);
            createdAtLabel.Name = "createdAtLabel";
            createdAtLabel.Size = new Size(54, 13);
            createdAtLabel.TabIndex = 23;
            createdAtLabel.Text = "Создана:";
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
            closeButton.Size = new Size(31, 31);
            closeButton.TabIndex = 3;
            closeButton.Text = "X";
            closeButton.UseVisualStyleBackColor = false;
            closeButton.Click += closeButton_Click;
            // 
            // editButton
            // 
            editButton.BackColor = Color.FromArgb(167, 157, 255);
            editButton.BackgroundImage = Resources.pencil;
            editButton.BackgroundImageLayout = ImageLayout.Zoom;
            editButton.Dock = DockStyle.Right;
            editButton.FlatAppearance.BorderColor = Color.FromArgb(228, 230, 240);
            editButton.FlatStyle = FlatStyle.Flat;
            editButton.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 204);
            editButton.ForeColor = Color.FromArgb(248, 249, 255);
            editButton.Location = new Point(153, 3);
            editButton.Name = "editButton";
            editButton.Size = new Size(31, 31);
            editButton.TabIndex = 17;
            editButton.UseMnemonic = false;
            editButton.UseVisualStyleBackColor = false;
            editButton.Click += editButton_Click;
            // 
            // hostNameLabel
            // 
            hostNameLabel.Anchor = AnchorStyles.Right;
            hostNameLabel.AutoSize = true;
            hostNameLabel.Font = new Font("Segoe UI", 8.25F);
            hostNameLabel.ForeColor = Color.Black;
            hostNameLabel.Location = new Point(57, 71);
            hostNameLabel.Name = "hostNameLabel";
            hostNameLabel.Size = new Size(33, 13);
            hostNameLabel.TabIndex = 18;
            hostNameLabel.Text = "Хост:";
            // 
            // hostName
            // 
            hostName.Anchor = AnchorStyles.Left;
            hostName.AutoSize = true;
            hostName.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            hostName.ForeColor = Color.FromArgb(0, 0, 0);
            hostName.Location = new Point(96, 69);
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
            notificationComboBox.Location = new Point(43, 41);
            notificationComboBox.Name = "notificationComboBox";
            notificationComboBox.Size = new Size(100, 19);
            notificationComboBox.TabIndex = 22;
            notificationComboBox.Text = "Уведомления";
            notificationComboBox.UseVisualStyleBackColor = false;
            // 
            // participantsLabel
            // 
            participantsLabel.Anchor = AnchorStyles.Bottom;
            participantsLabel.AutoSize = true;
            tableLayoutPanelStats.SetColumnSpan(participantsLabel, 2);
            participantsLabel.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            participantsLabel.ForeColor = Color.FromArgb(0, 0, 0);
            participantsLabel.Location = new Point(54, 253);
            participantsLabel.Name = "participantsLabel";
            participantsLabel.Size = new Size(78, 17);
            participantsLabel.TabIndex = 16;
            participantsLabel.Text = "Участники:";
            // 
            // computerLabel
            // 
            computerLabel.Anchor = AnchorStyles.Right;
            computerLabel.AutoSize = true;
            computerLabel.Font = new Font("Segoe UI", 8.25F);
            computerLabel.ForeColor = Color.Black;
            computerLabel.Location = new Point(31, 152);
            computerLabel.Name = "computerLabel";
            computerLabel.Size = new Size(59, 13);
            computerLabel.TabIndex = 20;
            computerLabel.Text = "№ Компа:";
            // 
            // computer
            // 
            computer.Anchor = AnchorStyles.Left;
            computer.AutoSize = true;
            computer.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            computer.ForeColor = Color.FromArgb(0, 0, 0);
            computer.Location = new Point(96, 150);
            computer.Name = "computer";
            computer.Size = new Size(74, 17);
            computer.TabIndex = 13;
            computer.Text = "Загрузка...";
            // 
            // classroomLabel
            // 
            classroomLabel.Anchor = AnchorStyles.Right;
            classroomLabel.AutoSize = true;
            classroomLabel.Font = new Font("Segoe UI", 8.25F);
            classroomLabel.ForeColor = Color.Black;
            classroomLabel.Location = new Point(36, 125);
            classroomLabel.Name = "classroomLabel";
            classroomLabel.Size = new Size(54, 13);
            classroomLabel.TabIndex = 19;
            classroomLabel.Text = "Кабинет:";
            // 
            // classroom
            // 
            classroom.Anchor = AnchorStyles.Left;
            classroom.AutoSize = true;
            classroom.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            classroom.ForeColor = Color.FromArgb(0, 0, 0);
            classroom.Location = new Point(96, 123);
            classroom.Name = "classroom";
            classroom.Size = new Size(74, 17);
            classroom.TabIndex = 12;
            classroom.Text = "Загрузка...";
            // 
            // onlineLabel
            // 
            onlineLabel.Anchor = AnchorStyles.Right;
            onlineLabel.AutoSize = true;
            onlineLabel.Font = new Font("Segoe UI", 8.25F);
            onlineLabel.ForeColor = Color.Black;
            onlineLabel.Location = new Point(48, 233);
            onlineLabel.Name = "onlineLabel";
            onlineLabel.Size = new Size(42, 13);
            onlineLabel.TabIndex = 21;
            onlineLabel.Text = "В сети:";
            // 
            // participantsInfo
            // 
            participantsInfo.Anchor = AnchorStyles.Left;
            participantsInfo.AutoSize = true;
            participantsInfo.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            participantsInfo.ForeColor = Color.FromArgb(0, 0, 0);
            participantsInfo.Location = new Point(96, 231);
            participantsInfo.Name = "participantsInfo";
            participantsInfo.Size = new Size(74, 17);
            participantsInfo.TabIndex = 15;
            participantsInfo.Text = "Загрузка...";
            // 
            // connectionAddress
            // 
            connectionAddress.Anchor = AnchorStyles.Left;
            connectionAddress.BackColor = Color.FromArgb(248, 249, 255);
            connectionAddress.BorderStyle = BorderStyle.None;
            connectionAddress.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 204);
            connectionAddress.ForeColor = Color.Black;
            connectionAddress.Location = new Point(96, 176);
            connectionAddress.Name = "connectionAddress";
            connectionAddress.ReadOnly = true;
            connectionAddress.Size = new Size(88, 18);
            connectionAddress.TabIndex = 29;
            connectionAddress.Text = "Загрузка...";
            // 
            // disconnectButton
            // 
            disconnectButton.BackColor = Color.FromArgb(248, 249, 255);
            disconnectButton.DialogResult = DialogResult.Cancel;
            disconnectButton.Dock = DockStyle.Fill;
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
            aboutButton.Dock = DockStyle.Fill;
            aboutButton.FlatAppearance.BorderColor = Color.FromArgb(228, 230, 240);
            aboutButton.FlatStyle = FlatStyle.Flat;
            aboutButton.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            aboutButton.ForeColor = Color.FromArgb(248, 249, 255);
            aboutButton.Location = new Point(776, 3);
            aboutButton.Name = "aboutButton";
            aboutButton.Size = new Size(42, 42);
            aboutButton.TabIndex = 4;
            aboutButton.Text = ". . .";
            aboutButton.UseVisualStyleBackColor = false;
            aboutButton.Click += aboutButton_Click;
            // 
            // tableLayoutPanelHeader
            // 
            tableLayoutPanelHeader.BackColor = Color.Transparent;
            tableLayoutPanelHeader.ColumnCount = 3;
            tableLayoutPanelHeader.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110F));
            tableLayoutPanelHeader.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanelHeader.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 48F));
            tableLayoutPanelHeader.Controls.Add(aboutButton, 2, 0);
            tableLayoutPanelHeader.Controls.Add(disconnectButton, 0, 0);
            tableLayoutPanelHeader.Controls.Add(Title, 1, 0);
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
            Title.Anchor = AnchorStyles.Left;
            Title.AutoSize = true;
            Title.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            Title.ForeColor = Color.Black;
            Title.Location = new Point(113, 9);
            Title.Name = "Title";
            Title.Size = new Size(104, 30);
            Title.TabIndex = 0;
            Title.Text = "Комната";
            // 
            // openFileDialog
            // 
            openFileDialog.Multiselect = true;
            openFileDialog.ShowPreview = true;
            // 
            // chatActionContextMenuStrip
            // 
            chatActionContextMenuStrip.Name = "chatActionContextMenuStrip";
            chatActionContextMenuStrip.Size = new Size(216, 48);
            chatActionContextMenuStrip.StartSessionClick = null;
            chatActionContextMenuStrip.UploadFileClick = null;
            // 
            // ChatPanelView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            MinimumSize = new Size(250, 298);
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
            tableLayoutPanelButtons.ResumeLayout(false);
            tableLayoutPanelButtons.PerformLayout();
            tableLayoutPanelStats.ResumeLayout(false);
            tableLayoutPanelStats.PerformLayout();
            tableLayoutPanelHeader.ResumeLayout(false);
            tableLayoutPanelHeader.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private SplitContainer splitContainerSideBar;
        private TableLayoutPanel tableLayoutPanelHeader;
        private Desktop.Components.Labels.Heading1Label Title;
        private Desktop.Components.Controls.FlowLayoutPanels.NoHorizontalScrollListView participantsFlow;
        private TableLayoutPanel tableLayoutPanelStats;
        private Desktop.Components.Labels.Heading3Label createdAt;
        private Desktop.Components.Labels.CaptionLabel createdAtLabel;
        private Desktop.Components.Controls.Buttons.InvertedButton closeButton;
        private Desktop.Components.Controls.Buttons.CommonButton editButton;
        private Desktop.Components.Labels.CaptionLabel hostNameLabel;
        private Desktop.Components.Labels.Heading3Label hostName;
        private Desktop.Components.Controls.CheckBoxes.DefaultCheckBox notificationComboBox;
        private Desktop.Components.Labels.Heading3Label participantsLabel;
        private Desktop.Components.Labels.CaptionLabel onlineLabel;
        private Desktop.Components.Labels.Heading3Label participantsInfo;
        private Desktop.Components.Labels.CaptionLabel computerLabel;
        private Desktop.Components.Labels.Heading3Label computer;
        private Desktop.Components.Labels.CaptionLabel classroomLabel;
        private Desktop.Components.Labels.Heading3Label classroom;
        private Desktop.Components.Controls.FlowLayoutPanels.NoHorizontalScrollListView chatFlow;
        private Desktop.Components.Controls.Buttons.InvertedButton disconnectButton;
        private Desktop.Components.Controls.Buttons.CommonButton aboutButton;
        private TableLayoutPanel tableLayoutPanelButtons;
        private Desktop.Components.Controls.Buttons.CommonButton actionButton;
        private Desktop.Components.Controls.Buttons.CommonButton sendButton;
        private Desktop.Components.Controls.TextBoxes.MessageTextBox messageTextBox;
        private OpenFileDialog openFileDialog;
        private Desktop.Components.ComplexControls.MultiFileAttachmentUploader multiFileAttachmentUploader;
        private Desktop.Components.Labels.CaptionLabel statusLabel;
        private Desktop.Components.Labels.CaptionLabel connectionAddressLabel;
        private Desktop.Components.Labels.CaptionLabel connectionPortLabel;
        private Desktop.Components.Textboxes.ReadonlyTextbox connectionAddress;
        private Desktop.Components.Textboxes.ReadonlyTextbox connectionPort;
        private Desktop.Components.Controls.ContextMenuStrips.ChatActionContextMenuStrip chatActionContextMenuStrip;
    }
}
