namespace MIN.Desktop.Views.Panels.SidePanelViews
{
    partial class DiscoveryPanelView
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
            tableLayoutPanel1 = new TableLayoutPanel();
            splitContainerDiscoverRoom = new SplitContainer();
            discoverRooms = new MIN.Desktop.Components.Controls.Buttons.CommonButton();
            discoveryProgressBar = new ProgressBar();
            ClassroomTitleInput = new MIN.Desktop.Components.Labels.Heading3Label();
            classNumber = new MIN.Desktop.Components.Controls.NumericUpDowns.DefaultNumericUpDown();
            flowLayoutPanelDiscoveredRooms = new FlowLayoutPanel();
            statusStrip = new StatusStrip();
            totalRoomsCount = new ToolStripStatusLabel();
            ((System.ComponentModel.ISupportInitialize)splitContainer).BeginInit();
            splitContainer.Panel1.SuspendLayout();
            splitContainer.Panel2.SuspendLayout();
            splitContainer.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainerDiscoverRoom).BeginInit();
            splitContainerDiscoverRoom.Panel1.SuspendLayout();
            splitContainerDiscoverRoom.Panel2.SuspendLayout();
            splitContainerDiscoverRoom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)classNumber).BeginInit();
            statusStrip.SuspendLayout();
            SuspendLayout();
            // 
            // splitContainer
            // 
            splitContainer.BackColor = Color.FromArgb(240, 242, 255);
            splitContainer.ForeColor = Color.FromArgb(45, 43, 58);
            // 
            // splitContainer.Panel1
            // 
            splitContainer.Panel1.Controls.Add(tableLayoutPanel1);
            // 
            // splitContainer.Panel2
            // 
            splitContainer.Panel2.Controls.Add(statusStrip);
            splitContainer.Panel2.Controls.Add(flowLayoutPanelDiscoveredRooms);
            splitContainer.Size = new Size(848, 591);
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 3;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 200F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 168F));
            tableLayoutPanel1.Controls.Add(splitContainerDiscoverRoom, 2, 0);
            tableLayoutPanel1.Controls.Add(ClassroomTitleInput, 0, 0);
            tableLayoutPanel1.Controls.Add(classNumber, 1, 0);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Margin = new Padding(0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 1;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
            tableLayoutPanel1.Size = new Size(848, 48);
            tableLayoutPanel1.TabIndex = 2;
            // 
            // splitContainerDiscoverRoom
            // 
            splitContainerDiscoverRoom.Dock = DockStyle.Fill;
            splitContainerDiscoverRoom.Location = new Point(683, 3);
            splitContainerDiscoverRoom.Name = "splitContainerDiscoverRoom";
            splitContainerDiscoverRoom.Orientation = Orientation.Horizontal;
            // 
            // splitContainerDiscoverRoom.Panel1
            // 
            splitContainerDiscoverRoom.Panel1.Controls.Add(discoverRooms);
            splitContainerDiscoverRoom.Panel1.Padding = new Padding(0, 0, 0, 3);
            // 
            // splitContainerDiscoverRoom.Panel2
            // 
            splitContainerDiscoverRoom.Panel2.Controls.Add(discoveryProgressBar);
            splitContainerDiscoverRoom.Panel2Collapsed = true;
            splitContainerDiscoverRoom.Size = new Size(162, 44);
            splitContainerDiscoverRoom.SplitterDistance = 25;
            splitContainerDiscoverRoom.TabIndex = 5;
            // 
            // discoverRooms
            // 
            discoverRooms.BackColor = Color.FromArgb(192, 192, 255);
            discoverRooms.Dock = DockStyle.Fill;
            discoverRooms.FlatAppearance.BorderColor = Color.FromArgb(228, 230, 240);
            discoverRooms.FlatStyle = FlatStyle.Flat;
            discoverRooms.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 204);
            discoverRooms.ForeColor = Color.FromArgb(248, 249, 255);
            discoverRooms.Location = new Point(0, 0);
            discoverRooms.Name = "discoverRooms";
            discoverRooms.Size = new Size(162, 47);
            discoverRooms.TabIndex = 2;
            discoverRooms.Text = "Найти комнаты";
            discoverRooms.UseVisualStyleBackColor = false;
            discoverRooms.Click += discoverRooms_Click;
            // 
            // discoveryProgressBar
            // 
            discoveryProgressBar.Dock = DockStyle.Bottom;
            discoveryProgressBar.Location = new Point(0, 38);
            discoveryProgressBar.Margin = new Padding(0);
            discoveryProgressBar.Name = "discoveryProgressBar";
            discoveryProgressBar.Size = new Size(150, 8);
            discoveryProgressBar.TabIndex = 0;
            // 
            // ClassroomTitleInput
            // 
            ClassroomTitleInput.Anchor = AnchorStyles.Right;
            ClassroomTitleInput.AutoSize = true;
            ClassroomTitleInput.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            ClassroomTitleInput.ForeColor = Color.FromArgb(0, 0, 0);
            ClassroomTitleInput.Location = new Point(3, 14);
            ClassroomTitleInput.Name = "ClassroomTitleInput";
            ClassroomTitleInput.Size = new Size(194, 21);
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
            classNumber.Location = new Point(203, 10);
            classNumber.Maximum = new decimal(new int[] { 440, 0, 0, 0 });
            classNumber.Minimum = new decimal(new int[] { 100, 0, 0, 0 });
            classNumber.Name = "classNumber";
            classNumber.Size = new Size(53, 29);
            classNumber.TabIndex = 1;
            classNumber.Value = new decimal(new int[] { 100, 0, 0, 0 });
            // 
            // flowLayoutPanelDiscoveredRooms
            // 
            flowLayoutPanelDiscoveredRooms.AutoScroll = true;
            flowLayoutPanelDiscoveredRooms.Dock = DockStyle.Fill;
            flowLayoutPanelDiscoveredRooms.Location = new Point(0, 0);
            flowLayoutPanelDiscoveredRooms.Margin = new Padding(20);
            flowLayoutPanelDiscoveredRooms.Name = "flowLayoutPanelDiscoveredRooms";
            flowLayoutPanelDiscoveredRooms.Size = new Size(848, 541);
            flowLayoutPanelDiscoveredRooms.TabIndex = 1;
            // 
            // statusStrip
            // 
            statusStrip.ImageScalingSize = new Size(20, 20);
            statusStrip.Items.AddRange(new ToolStripItem[] { totalRoomsCount });
            statusStrip.Location = new Point(0, 519);
            statusStrip.Name = "statusStrip";
            statusStrip.Size = new Size(848, 22);
            statusStrip.TabIndex = 2;
            statusStrip.Text = "statusStrip1";
            // 
            // totalRoomsCount
            // 
            totalRoomsCount.Name = "totalRoomsCount";
            totalRoomsCount.Size = new Size(146, 17);
            totalRoomsCount.Text = "Всего нашлось комнат: 0";
            // 
            // DiscoveryPanelView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            MinimumSize = new Size(429, 306);
            Name = "DiscoveryPanelView";
            Size = new Size(848, 591);
            Controls.SetChildIndex(splitContainer, 0);
            splitContainer.Panel1.ResumeLayout(false);
            splitContainer.Panel2.ResumeLayout(false);
            splitContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer).EndInit();
            splitContainer.ResumeLayout(false);
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            splitContainerDiscoverRoom.Panel1.ResumeLayout(false);
            splitContainerDiscoverRoom.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerDiscoverRoom).EndInit();
            splitContainerDiscoverRoom.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)classNumber).EndInit();
            statusStrip.ResumeLayout(false);
            statusStrip.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanel1;
        private Desktop.Components.Labels.Heading3Label ClassroomTitleInput;
        private Desktop.Components.Controls.NumericUpDowns.DefaultNumericUpDown classNumber;
        private SplitContainer splitContainerDiscoverRoom;
        private Desktop.Components.Controls.Buttons.CommonButton discoverRooms;
        private ProgressBar discoveryProgressBar;
        private FlowLayoutPanel flowLayoutPanelDiscoveredRooms;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel totalRoomsCount;
    }
}
