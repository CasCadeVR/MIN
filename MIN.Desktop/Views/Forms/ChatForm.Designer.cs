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
            splitContainerSideBar = new SplitContainer();
            splitContainer = new SplitContainer();
            tableLayoutPanelHeader = new TableLayoutPanel();
            Title = new MIN.Desktop.Components.Labels.Heading1Label();
            chatFlow = new FlowLayoutPanel();
            tableLayoutPanelButtons = new TableLayoutPanel();
            fileButton = new MIN.Desktop.Components.CommonButton();
            sendButton = new MIN.Desktop.Components.CommonButton();
            messageTextBox = new MIN.Desktop.Components.Controls.TextBoxes.DefaultTextBox();
            tableLayoutPanel1 = new TableLayoutPanel();
            disconnectButton = new MIN.Desktop.Components.InvertedButton();
            aboutButton = new MIN.Desktop.Components.CommonButton();
            splitContainer1 = new SplitContainer();
            tableLayoutPanel2 = new TableLayoutPanel();
            TitleSideBar = new MIN.Desktop.Components.Labels.Heading1Label();
            participantsFlow = new FlowLayoutPanel();
            tableLayoutPanelStats = new TableLayoutPanel();
            heading3Label4 = new MIN.Desktop.Components.Labels.Heading3Label();
            classroom = new MIN.Desktop.Components.Labels.Heading3Label();
            closeButton = new MIN.Desktop.Components.InvertedButton();
            ClassTitleInput = new MIN.Desktop.Components.Labels.Heading3Label();
            heading3Label2 = new MIN.Desktop.Components.Labels.Heading3Label();
            heading3Label3 = new MIN.Desktop.Components.Labels.Heading3Label();
            hostName = new MIN.Desktop.Components.Labels.Heading3Label();
            participantsInfo = new MIN.Desktop.Components.Labels.Heading3Label();
            computer = new MIN.Desktop.Components.Labels.Heading3Label();
            heading3Label1 = new MIN.Desktop.Components.Labels.Heading3Label();
            ((System.ComponentModel.ISupportInitialize)splitContainerSideBar).BeginInit();
            splitContainerSideBar.Panel1.SuspendLayout();
            splitContainerSideBar.Panel2.SuspendLayout();
            splitContainerSideBar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer).BeginInit();
            splitContainer.Panel1.SuspendLayout();
            splitContainer.Panel2.SuspendLayout();
            splitContainer.SuspendLayout();
            tableLayoutPanelHeader.SuspendLayout();
            tableLayoutPanelButtons.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            tableLayoutPanelStats.SuspendLayout();
            SuspendLayout();
            // 
            // splitContainerSideBar
            // 
            splitContainerSideBar.Dock = DockStyle.Fill;
            splitContainerSideBar.Location = new Point(0, 0);
            splitContainerSideBar.Name = "splitContainerSideBar";
            // 
            // splitContainerSideBar.Panel1
            // 
            splitContainerSideBar.Panel1.Controls.Add(splitContainer);
            // 
            // splitContainerSideBar.Panel2
            // 
            splitContainerSideBar.Panel2.Controls.Add(splitContainer1);
            splitContainerSideBar.Size = new Size(416, 571);
            splitContainerSideBar.SplitterDistance = 209;
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
            splitContainer.Panel2.Controls.Add(chatFlow);
            splitContainer.Panel2.Controls.Add(tableLayoutPanelButtons);
            splitContainer.Panel2.Controls.Add(tableLayoutPanel1);
            splitContainer.Size = new Size(209, 571);
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
            tableLayoutPanelHeader.Size = new Size(209, 55);
            tableLayoutPanelHeader.TabIndex = 0;
            // 
            // Title
            // 
            Title.Anchor = AnchorStyles.None;
            Title.AutoSize = true;
            Title.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            Title.ForeColor = Color.FromArgb(248, 249, 255);
            Title.Location = new Point(38, 12);
            Title.Name = "Title";
            Title.Size = new Size(132, 30);
            Title.TabIndex = 0;
            Title.Text = "Комната \"\"";
            // 
            // chatFlow
            // 
            chatFlow.AutoScroll = true;
            chatFlow.Dock = DockStyle.Fill;
            chatFlow.FlowDirection = FlowDirection.BottomUp;
            chatFlow.Location = new Point(0, 48);
            chatFlow.Name = "chatFlow";
            chatFlow.Size = new Size(209, 416);
            chatFlow.TabIndex = 3;
            chatFlow.WrapContents = false;
            // 
            // tableLayoutPanelButtons
            // 
            tableLayoutPanelButtons.ColumnCount = 3;
            tableLayoutPanelButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 116F));
            tableLayoutPanelButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanelButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 48F));
            tableLayoutPanelButtons.Controls.Add(fileButton, 0, 0);
            tableLayoutPanelButtons.Controls.Add(sendButton, 2, 0);
            tableLayoutPanelButtons.Controls.Add(messageTextBox, 1, 0);
            tableLayoutPanelButtons.Dock = DockStyle.Bottom;
            tableLayoutPanelButtons.Location = new Point(0, 464);
            tableLayoutPanelButtons.Margin = new Padding(0);
            tableLayoutPanelButtons.Name = "tableLayoutPanelButtons";
            tableLayoutPanelButtons.RowCount = 1;
            tableLayoutPanelButtons.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanelButtons.Size = new Size(209, 48);
            tableLayoutPanelButtons.TabIndex = 2;
            // 
            // fileButton
            // 
            fileButton.BackColor = Color.FromArgb(192, 192, 255);
            fileButton.Dock = DockStyle.Left;
            fileButton.FlatAppearance.BorderColor = Color.FromArgb(228, 230, 240);
            fileButton.FlatStyle = FlatStyle.Flat;
            fileButton.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 204);
            fileButton.ForeColor = Color.FromArgb(248, 249, 255);
            fileButton.Location = new Point(3, 3);
            fileButton.Name = "fileButton";
            fileButton.Padding = new Padding(8, 4, 8, 4);
            fileButton.Size = new Size(110, 42);
            fileButton.TabIndex = 3;
            fileButton.Text = "Прикрепить";
            fileButton.UseVisualStyleBackColor = false;
            // 
            // sendButton
            // 
            sendButton.BackColor = Color.FromArgb(192, 192, 255);
            sendButton.Dock = DockStyle.Right;
            sendButton.FlatAppearance.BorderColor = Color.FromArgb(228, 230, 240);
            sendButton.FlatStyle = FlatStyle.Flat;
            sendButton.Font = new Font("Segoe UI Black", 9F, FontStyle.Bold, GraphicsUnit.Point, 204);
            sendButton.ForeColor = Color.FromArgb(248, 249, 255);
            sendButton.Location = new Point(164, 3);
            sendButton.Name = "sendButton";
            sendButton.Padding = new Padding(8, 4, 8, 4);
            sendButton.Size = new Size(42, 42);
            sendButton.TabIndex = 2;
            sendButton.Text = ">";
            sendButton.UseVisualStyleBackColor = false;
            // 
            // messageTextBox
            // 
            messageTextBox.BackColor = Color.FromArgb(248, 249, 255);
            messageTextBox.BorderStyle = BorderStyle.None;
            messageTextBox.Dock = DockStyle.Fill;
            messageTextBox.Font = new Font("Segoe UI", 9.75F);
            messageTextBox.ForeColor = Color.FromArgb(122, 119, 143);
            messageTextBox.Location = new Point(119, 3);
            messageTextBox.Multiline = true;
            messageTextBox.Name = "messageTextBox";
            messageTextBox.PlaceholderText = "Message";
            messageTextBox.Size = new Size(39, 42);
            messageTextBox.TabIndex = 4;
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
            tableLayoutPanel1.Size = new Size(209, 48);
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
            disconnectButton.Size = new Size(98, 42);
            disconnectButton.TabIndex = 3;
            disconnectButton.Text = "Отключиться";
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
            aboutButton.Location = new Point(107, 3);
            aboutButton.Name = "aboutButton";
            aboutButton.Padding = new Padding(8, 4, 8, 4);
            aboutButton.Size = new Size(99, 42);
            aboutButton.TabIndex = 4;
            aboutButton.Text = "О комнате";
            aboutButton.UseVisualStyleBackColor = false;
            aboutButton.Click += aboutButton_Click;
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.FixedPanel = FixedPanel.Panel1;
            splitContainer1.IsSplitterFixed = true;
            splitContainer1.Location = new Point(0, 0);
            splitContainer1.Name = "splitContainer1";
            splitContainer1.Orientation = Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(tableLayoutPanel2);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(participantsFlow);
            splitContainer1.Panel2.Controls.Add(tableLayoutPanelStats);
            splitContainer1.Size = new Size(206, 571);
            splitContainer1.SplitterDistance = 55;
            splitContainer1.TabIndex = 2;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 1;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel2.Controls.Add(TitleSideBar, 0, 0);
            tableLayoutPanel2.Dock = DockStyle.Fill;
            tableLayoutPanel2.Location = new Point(0, 0);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 1;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.Size = new Size(206, 55);
            tableLayoutPanel2.TabIndex = 0;
            // 
            // TitleSideBar
            // 
            TitleSideBar.Anchor = AnchorStyles.None;
            TitleSideBar.AutoSize = true;
            TitleSideBar.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            TitleSideBar.ForeColor = Color.FromArgb(248, 249, 255);
            TitleSideBar.Location = new Point(37, 12);
            TitleSideBar.Name = "TitleSideBar";
            TitleSideBar.Size = new Size(132, 30);
            TitleSideBar.TabIndex = 0;
            TitleSideBar.Text = "Комната \"\"";
            // 
            // participantsFlow
            // 
            participantsFlow.AutoScroll = true;
            participantsFlow.Dock = DockStyle.Fill;
            participantsFlow.FlowDirection = FlowDirection.BottomUp;
            participantsFlow.Location = new Point(0, 290);
            participantsFlow.Name = "participantsFlow";
            participantsFlow.Size = new Size(206, 222);
            participantsFlow.TabIndex = 3;
            participantsFlow.WrapContents = false;
            // 
            // tableLayoutPanelStats
            // 
            tableLayoutPanelStats.ColumnCount = 2;
            tableLayoutPanelStats.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanelStats.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanelStats.Controls.Add(heading3Label4, 0, 5);
            tableLayoutPanelStats.Controls.Add(classroom, 1, 2);
            tableLayoutPanelStats.Controls.Add(closeButton, 0, 0);
            tableLayoutPanelStats.Controls.Add(ClassTitleInput, 0, 1);
            tableLayoutPanelStats.Controls.Add(heading3Label2, 0, 3);
            tableLayoutPanelStats.Controls.Add(heading3Label3, 0, 4);
            tableLayoutPanelStats.Controls.Add(hostName, 1, 1);
            tableLayoutPanelStats.Controls.Add(participantsInfo, 1, 4);
            tableLayoutPanelStats.Controls.Add(computer, 1, 3);
            tableLayoutPanelStats.Controls.Add(heading3Label1, 0, 2);
            tableLayoutPanelStats.Dock = DockStyle.Top;
            tableLayoutPanelStats.Location = new Point(0, 0);
            tableLayoutPanelStats.Margin = new Padding(0);
            tableLayoutPanelStats.Name = "tableLayoutPanelStats";
            tableLayoutPanelStats.RowCount = 6;
            tableLayoutPanelStats.RowStyles.Add(new RowStyle(SizeType.Percent, 16.6666679F));
            tableLayoutPanelStats.RowStyles.Add(new RowStyle(SizeType.Percent, 16.6666679F));
            tableLayoutPanelStats.RowStyles.Add(new RowStyle(SizeType.Percent, 16.6666679F));
            tableLayoutPanelStats.RowStyles.Add(new RowStyle(SizeType.Percent, 16.6666679F));
            tableLayoutPanelStats.RowStyles.Add(new RowStyle(SizeType.Percent, 16.6666679F));
            tableLayoutPanelStats.RowStyles.Add(new RowStyle(SizeType.Percent, 16.6666679F));
            tableLayoutPanelStats.Size = new Size(206, 290);
            tableLayoutPanelStats.TabIndex = 1;
            // 
            // heading3Label4
            // 
            heading3Label4.Anchor = AnchorStyles.Bottom;
            heading3Label4.AutoSize = true;
            tableLayoutPanelStats.SetColumnSpan(heading3Label4, 2);
            heading3Label4.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            heading3Label4.ForeColor = Color.FromArgb(0, 0, 0);
            heading3Label4.Location = new Point(64, 273);
            heading3Label4.Name = "heading3Label4";
            heading3Label4.Size = new Size(78, 17);
            heading3Label4.TabIndex = 16;
            heading3Label4.Text = "Участники:";
            // 
            // classroom
            // 
            classroom.Anchor = AnchorStyles.Left;
            classroom.AutoSize = true;
            classroom.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            classroom.ForeColor = Color.FromArgb(0, 0, 0);
            classroom.Location = new Point(106, 111);
            classroom.Name = "classroom";
            classroom.Size = new Size(74, 17);
            classroom.TabIndex = 12;
            classroom.Text = "Загрузка...";
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
            closeButton.Padding = new Padding(8, 4, 8, 4);
            closeButton.Size = new Size(97, 42);
            closeButton.TabIndex = 3;
            closeButton.Text = "Закрыть";
            closeButton.UseVisualStyleBackColor = false;
            closeButton.Click += closeButton_Click;
            // 
            // ClassTitleInput
            // 
            ClassTitleInput.Anchor = AnchorStyles.Right;
            ClassTitleInput.AutoSize = true;
            ClassTitleInput.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            ClassTitleInput.ForeColor = Color.FromArgb(0, 0, 0);
            ClassTitleInput.Location = new Point(23, 55);
            ClassTitleInput.Name = "ClassTitleInput";
            ClassTitleInput.Size = new Size(77, 34);
            ClassTitleInput.TabIndex = 8;
            ClassTitleInput.Text = "Создатель комнаты: ";
            // 
            // heading3Label2
            // 
            heading3Label2.Anchor = AnchorStyles.Right;
            heading3Label2.AutoSize = true;
            heading3Label2.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            heading3Label2.ForeColor = Color.FromArgb(0, 0, 0);
            heading3Label2.Location = new Point(4, 151);
            heading3Label2.Name = "heading3Label2";
            heading3Label2.Size = new Size(96, 34);
            heading3Label2.TabIndex = 11;
            heading3Label2.Text = "№ Компьютера: ";
            // 
            // heading3Label3
            // 
            heading3Label3.Anchor = AnchorStyles.Right;
            heading3Label3.AutoSize = true;
            heading3Label3.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            heading3Label3.ForeColor = Color.FromArgb(0, 0, 0);
            heading3Label3.Location = new Point(5, 207);
            heading3Label3.Name = "heading3Label3";
            heading3Label3.Size = new Size(95, 17);
            heading3Label3.TabIndex = 14;
            heading3Label3.Text = "Подключено: ";
            // 
            // hostName
            // 
            hostName.Anchor = AnchorStyles.Left;
            hostName.AutoSize = true;
            hostName.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            hostName.ForeColor = Color.FromArgb(0, 0, 0);
            hostName.Location = new Point(106, 63);
            hostName.Name = "hostName";
            hostName.Size = new Size(74, 17);
            hostName.TabIndex = 9;
            hostName.Text = "Загрузка...";
            // 
            // participantsInfo
            // 
            participantsInfo.Anchor = AnchorStyles.Left;
            participantsInfo.AutoSize = true;
            participantsInfo.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            participantsInfo.ForeColor = Color.FromArgb(0, 0, 0);
            participantsInfo.Location = new Point(106, 207);
            participantsInfo.Name = "participantsInfo";
            participantsInfo.Size = new Size(74, 17);
            participantsInfo.TabIndex = 15;
            participantsInfo.Text = "Загрузка...";
            // 
            // computer
            // 
            computer.Anchor = AnchorStyles.Left;
            computer.AutoSize = true;
            computer.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            computer.ForeColor = Color.FromArgb(0, 0, 0);
            computer.Location = new Point(106, 159);
            computer.Name = "computer";
            computer.Size = new Size(74, 17);
            computer.TabIndex = 13;
            computer.Text = "Загрузка...";
            // 
            // heading3Label1
            // 
            heading3Label1.Anchor = AnchorStyles.Right;
            heading3Label1.AutoSize = true;
            heading3Label1.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            heading3Label1.ForeColor = Color.FromArgb(0, 0, 0);
            heading3Label1.Location = new Point(32, 111);
            heading3Label1.Name = "heading3Label1";
            heading3Label1.Size = new Size(68, 17);
            heading3Label1.TabIndex = 10;
            heading3Label1.Text = "Кабинет: ";
            // 
            // ChatForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(416, 571);
            Controls.Add(splitContainerSideBar);
            MinimumSize = new Size(432, 241);
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
            tableLayoutPanelButtons.ResumeLayout(false);
            tableLayoutPanelButtons.PerformLayout();
            tableLayoutPanel1.ResumeLayout(false);
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            tableLayoutPanel2.ResumeLayout(false);
            tableLayoutPanel2.PerformLayout();
            tableLayoutPanelStats.ResumeLayout(false);
            tableLayoutPanelStats.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        private Components.Controls.NumericUpDowns.DefaultNumericUpDown classNumber;
        private SplitContainer splitContainerSideBar;
        private SplitContainer splitContainer;
        private TableLayoutPanel tableLayoutPanelHeader;
        private Components.Labels.Heading1Label Title;
        private FlowLayoutPanel chatFlow;
        private TableLayoutPanel tableLayoutPanelButtons;
        private Components.CommonButton fileButton;
        private Components.CommonButton sendButton;
        private Components.Controls.TextBoxes.DefaultTextBox messageTextBox;
        private TableLayoutPanel tableLayoutPanel1;
        private Components.InvertedButton disconnectButton;
        private Components.CommonButton aboutButton;
        private SplitContainer splitContainer1;
        private TableLayoutPanel tableLayoutPanel2;
        private Components.Labels.Heading1Label TitleSideBar;
        private FlowLayoutPanel participantsFlow;
        private TableLayoutPanel tableLayoutPanelStats;
        private Components.InvertedButton closeButton;
        private Components.Labels.Heading3Label heading3Label4;
        private Components.Labels.Heading3Label classroom;
        private Components.Labels.Heading3Label ClassTitleInput;
        private Components.Labels.Heading3Label heading3Label2;
        private Components.Labels.Heading3Label heading3Label3;
        private Components.Labels.Heading3Label hostName;
        private Components.Labels.Heading3Label participantsInfo;
        private Components.Labels.Heading3Label computer;
        private Components.Labels.Heading3Label heading3Label1;
    }
}
