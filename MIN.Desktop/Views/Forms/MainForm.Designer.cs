namespace MIN.Desktop
{
    partial class MainForm
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
            splitContainer = new SplitContainer();
            tableLayoutPanelHeader = new TableLayoutPanel();
            Title = new MIN.Desktop.Components.Labels.Heading1Label();
            splitContainerClass = new SplitContainer();
            tableLayoutPanelMainButtons = new TableLayoutPanel();
            findRooms = new MIN.Desktop.Components.CommonButton();
            createRoom = new MIN.Desktop.Components.CommonButton();
            tableLayoutPanel1 = new TableLayoutPanel();
            ClassroomTitleInput = new MIN.Desktop.Components.Labels.Heading3Label();
            classNumber = new MIN.Desktop.Components.Controls.NumericUpDowns.DefaultNumericUpDown();
            settingsButton = new MIN.Desktop.Components.CommonButton();
            statusStrip = new StatusStrip();
            totalRoomsCount = new ToolStripStatusLabel();
            flowLayoutPanel = new FlowLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)splitContainer).BeginInit();
            splitContainer.Panel1.SuspendLayout();
            splitContainer.Panel2.SuspendLayout();
            splitContainer.SuspendLayout();
            tableLayoutPanelHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainerClass).BeginInit();
            splitContainerClass.Panel1.SuspendLayout();
            splitContainerClass.Panel2.SuspendLayout();
            splitContainerClass.SuspendLayout();
            tableLayoutPanelMainButtons.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)classNumber).BeginInit();
            statusStrip.SuspendLayout();
            SuspendLayout();
            // 
            // splitContainer
            // 
            splitContainer.Dock = DockStyle.Fill;
            splitContainer.FixedPanel = FixedPanel.Panel1;
            splitContainer.IsSplitterFixed = true;
            splitContainer.Location = new Point(0, 0);
            splitContainer.Margin = new Padding(3, 4, 3, 4);
            splitContainer.Name = "splitContainer";
            splitContainer.Orientation = Orientation.Horizontal;
            // 
            // splitContainer.Panel1
            // 
            splitContainer.Panel1.Controls.Add(tableLayoutPanelHeader);
            // 
            // splitContainer.Panel2
            // 
            splitContainer.Panel2.Controls.Add(splitContainerClass);
            splitContainer.Size = new Size(914, 600);
            splitContainer.SplitterDistance = 73;
            splitContainer.SplitterWidth = 5;
            splitContainer.TabIndex = 0;
            // 
            // tableLayoutPanelHeader
            // 
            tableLayoutPanelHeader.ColumnCount = 1;
            tableLayoutPanelHeader.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanelHeader.Controls.Add(Title, 0, 0);
            tableLayoutPanelHeader.Dock = DockStyle.Fill;
            tableLayoutPanelHeader.Location = new Point(0, 0);
            tableLayoutPanelHeader.Margin = new Padding(3, 4, 3, 4);
            tableLayoutPanelHeader.Name = "tableLayoutPanelHeader";
            tableLayoutPanelHeader.RowCount = 1;
            tableLayoutPanelHeader.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanelHeader.Size = new Size(914, 73);
            tableLayoutPanelHeader.TabIndex = 0;
            // 
            // Title
            // 
            Title.Anchor = AnchorStyles.None;
            Title.AutoSize = true;
            Title.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            Title.ForeColor = Color.FromArgb(248, 249, 255);
            Title.Location = new Point(420, 18);
            Title.Name = "Title";
            Title.Size = new Size(73, 37);
            Title.TabIndex = 0;
            Title.Text = "MIN";
            // 
            // splitContainerClass
            // 
            splitContainerClass.Dock = DockStyle.Fill;
            splitContainerClass.FixedPanel = FixedPanel.Panel1;
            splitContainerClass.IsSplitterFixed = true;
            splitContainerClass.Location = new Point(0, 0);
            splitContainerClass.Margin = new Padding(3, 4, 3, 4);
            splitContainerClass.Name = "splitContainerClass";
            splitContainerClass.Orientation = Orientation.Horizontal;
            // 
            // splitContainerClass.Panel1
            // 
            splitContainerClass.Panel1.Controls.Add(tableLayoutPanelMainButtons);
            splitContainerClass.Panel1.Controls.Add(tableLayoutPanel1);
            // 
            // splitContainerClass.Panel2
            // 
            splitContainerClass.Panel2.Controls.Add(statusStrip);
            splitContainerClass.Panel2.Controls.Add(flowLayoutPanel);
            splitContainerClass.Size = new Size(914, 522);
            splitContainerClass.SplitterDistance = 67;
            splitContainerClass.SplitterWidth = 5;
            splitContainerClass.TabIndex = 0;
            // 
            // tableLayoutPanelMainButtons
            // 
            tableLayoutPanelMainButtons.ColumnCount = 2;
            tableLayoutPanelMainButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanelMainButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanelMainButtons.Controls.Add(findRooms, 0, 0);
            tableLayoutPanelMainButtons.Controls.Add(createRoom, 1, 0);
            tableLayoutPanelMainButtons.Dock = DockStyle.Right;
            tableLayoutPanelMainButtons.Location = new Point(459, 0);
            tableLayoutPanelMainButtons.Margin = new Padding(0);
            tableLayoutPanelMainButtons.Name = "tableLayoutPanelMainButtons";
            tableLayoutPanelMainButtons.RowCount = 1;
            tableLayoutPanelMainButtons.RowStyles.Add(new RowStyle(SizeType.Absolute, 67F));
            tableLayoutPanelMainButtons.Size = new Size(455, 67);
            tableLayoutPanelMainButtons.TabIndex = 2;
            // 
            // findRooms
            // 
            findRooms.Anchor = AnchorStyles.Right;
            findRooms.BackColor = Color.FromArgb(192, 192, 255);
            findRooms.FlatAppearance.BorderColor = Color.FromArgb(228, 230, 240);
            findRooms.FlatStyle = FlatStyle.Flat;
            findRooms.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 204);
            findRooms.ForeColor = Color.FromArgb(248, 249, 255);
            findRooms.Location = new Point(3, 4);
            findRooms.Margin = new Padding(3, 4, 3, 4);
            findRooms.Name = "findRooms";
            findRooms.Padding = new Padding(9, 5, 9, 5);
            findRooms.Size = new Size(221, 59);
            findRooms.TabIndex = 2;
            findRooms.Text = "Найти комнаты";
            findRooms.UseVisualStyleBackColor = false;
            findRooms.Click += findRooms_Click;
            // 
            // createRoom
            // 
            createRoom.Anchor = AnchorStyles.Left;
            createRoom.BackColor = Color.FromArgb(192, 192, 255);
            createRoom.FlatAppearance.BorderColor = Color.FromArgb(228, 230, 240);
            createRoom.FlatStyle = FlatStyle.Flat;
            createRoom.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 204);
            createRoom.ForeColor = Color.FromArgb(248, 249, 255);
            createRoom.Location = new Point(230, 4);
            createRoom.Margin = new Padding(3, 4, 3, 4);
            createRoom.Name = "createRoom";
            createRoom.Padding = new Padding(9, 5, 9, 5);
            createRoom.Size = new Size(221, 59);
            createRoom.TabIndex = 3;
            createRoom.Text = "Создать комнату";
            createRoom.UseVisualStyleBackColor = false;
            createRoom.Click += createRoom_Click;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 3;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 57F));
            tableLayoutPanel1.Controls.Add(ClassroomTitleInput, 0, 0);
            tableLayoutPanel1.Controls.Add(classNumber, 1, 0);
            tableLayoutPanel1.Controls.Add(settingsButton, 2, 0);
            tableLayoutPanel1.Dock = DockStyle.Left;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Margin = new Padding(0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 1;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 67F));
            tableLayoutPanel1.Size = new Size(459, 67);
            tableLayoutPanel1.TabIndex = 1;
            // 
            // ClassroomTitleInput
            // 
            ClassroomTitleInput.Anchor = AnchorStyles.Right;
            ClassroomTitleInput.AutoSize = true;
            ClassroomTitleInput.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            ClassroomTitleInput.ForeColor = Color.FromArgb(0, 0, 0);
            ClassroomTitleInput.Location = new Point(50, 5);
            ClassroomTitleInput.Name = "ClassroomTitleInput";
            ClassroomTitleInput.Size = new Size(148, 56);
            ClassroomTitleInput.TabIndex = 0;
            ClassroomTitleInput.Text = "Введи номер кабинета: ";
            // 
            // classNumber
            // 
            classNumber.Anchor = AnchorStyles.Left;
            classNumber.BackColor = Color.White;
            classNumber.BorderStyle = BorderStyle.None;
            classNumber.Font = new Font("Segoe UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            classNumber.ForeColor = Color.Purple;
            classNumber.Location = new Point(204, 16);
            classNumber.Margin = new Padding(3, 4, 3, 4);
            classNumber.Maximum = new decimal(new int[] { 440, 0, 0, 0 });
            classNumber.Minimum = new decimal(new int[] { 100, 0, 0, 0 });
            classNumber.Name = "classNumber";
            classNumber.Size = new Size(61, 35);
            classNumber.TabIndex = 1;
            classNumber.Value = new decimal(new int[] { 100, 0, 0, 0 });
            // 
            // settingsButton
            // 
            settingsButton.BackColor = Color.FromArgb(167, 157, 255);
            settingsButton.BackgroundImage = Properties.Resources.settings;
            settingsButton.BackgroundImageLayout = ImageLayout.Zoom;
            settingsButton.Dock = DockStyle.Fill;
            settingsButton.FlatAppearance.BorderColor = Color.FromArgb(228, 230, 240);
            settingsButton.FlatStyle = FlatStyle.Flat;
            settingsButton.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            settingsButton.ForeColor = Color.FromArgb(248, 249, 255);
            settingsButton.Location = new Point(405, 4);
            settingsButton.Margin = new Padding(3, 4, 3, 4);
            settingsButton.Name = "settingsButton";
            settingsButton.Padding = new Padding(9, 5, 9, 5);
            settingsButton.Size = new Size(51, 59);
            settingsButton.TabIndex = 2;
            settingsButton.UseVisualStyleBackColor = false;
            settingsButton.Click += settingsButton_Click;
            // 
            // statusStrip
            // 
            statusStrip.ImageScalingSize = new Size(20, 20);
            statusStrip.Items.AddRange(new ToolStripItem[] { totalRoomsCount });
            statusStrip.Location = new Point(0, 424);
            statusStrip.Name = "statusStrip";
            statusStrip.Padding = new Padding(1, 0, 16, 0);
            statusStrip.Size = new Size(914, 26);
            statusStrip.TabIndex = 1;
            statusStrip.Text = "statusStrip1";
            // 
            // totalRoomsCount
            // 
            totalRoomsCount.Name = "totalRoomsCount";
            totalRoomsCount.Size = new Size(182, 20);
            totalRoomsCount.Text = "Всего нашлось комнат: 0";
            // 
            // flowLayoutPanel
            // 
            flowLayoutPanel.Dock = DockStyle.Fill;
            flowLayoutPanel.Location = new Point(0, 0);
            flowLayoutPanel.Margin = new Padding(23, 27, 23, 27);
            flowLayoutPanel.Name = "flowLayoutPanel";
            flowLayoutPanel.Size = new Size(914, 450);
            flowLayoutPanel.TabIndex = 0;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(914, 600);
            Controls.Add(splitContainer);
            Margin = new Padding(3, 5, 3, 5);
            MinimumSize = new Size(930, 636);
            Name = "MainForm";
            Text = "MIN";
            Load += MainForm_Load;
            splitContainer.Panel1.ResumeLayout(false);
            splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer).EndInit();
            splitContainer.ResumeLayout(false);
            tableLayoutPanelHeader.ResumeLayout(false);
            tableLayoutPanelHeader.PerformLayout();
            splitContainerClass.Panel1.ResumeLayout(false);
            splitContainerClass.Panel2.ResumeLayout(false);
            splitContainerClass.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainerClass).EndInit();
            splitContainerClass.ResumeLayout(false);
            tableLayoutPanelMainButtons.ResumeLayout(false);
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)classNumber).EndInit();
            statusStrip.ResumeLayout(false);
            statusStrip.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private SplitContainer splitContainer;
        private TableLayoutPanel tableLayoutPanelHeader;
        private SplitContainer splitContainerClass;
        private TableLayoutPanel tableLayoutPanel1;
        private Components.Labels.Heading3Label ClassroomTitleInput;
        private Components.Labels.Heading1Label Title;
        private FlowLayoutPanel flowLayoutPanel;
        private Components.Controls.NumericUpDowns.DefaultNumericUpDown classNumber;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel totalRoomsCount;
        private TableLayoutPanel tableLayoutPanelMainButtons;
        private Components.CommonButton findRooms;
        private Components.CommonButton createRoom;
        private Components.CommonButton settingsButton;
    }
}
