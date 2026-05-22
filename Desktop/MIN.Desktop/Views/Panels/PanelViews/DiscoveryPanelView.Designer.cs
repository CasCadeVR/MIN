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
            tableLayoutPanel = new TableLayoutPanel();
            createRoom = new MIN.Desktop.Components.Controls.Buttons.CommonButton();
            splitContainerDiscoverRoom = new SplitContainer();
            discoverRooms = new MIN.Desktop.Components.Controls.Buttons.InvertedButton();
            discoveryProgressBar = new ProgressBar();
            flowLayoutPanelDiscoveredRooms = new FlowLayoutPanel();
            statusStrip = new StatusStrip();
            totalRoomsCount = new ToolStripStatusLabel();
            ((System.ComponentModel.ISupportInitialize)splitContainer).BeginInit();
            splitContainer.Panel1.SuspendLayout();
            splitContainer.Panel2.SuspendLayout();
            splitContainer.SuspendLayout();
            tableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainerDiscoverRoom).BeginInit();
            splitContainerDiscoverRoom.Panel1.SuspendLayout();
            splitContainerDiscoverRoom.Panel2.SuspendLayout();
            splitContainerDiscoverRoom.SuspendLayout();
            statusStrip.SuspendLayout();
            SuspendLayout();
            // 
            // splitContainer
            // 
            splitContainer.BackColor = Color.Transparent;
            splitContainer.ForeColor = Color.FromArgb(45, 43, 58);
            // 
            // splitContainer.Panel1
            // 
            splitContainer.Panel1.Controls.Add(tableLayoutPanel);
            // 
            // splitContainer.Panel2
            // 
            splitContainer.Panel2.Controls.Add(statusStrip);
            splitContainer.Panel2.Controls.Add(flowLayoutPanelDiscoveredRooms);
            splitContainer.Size = new Size(836, 591);
            // 
            // tableLayoutPanel
            // 
            tableLayoutPanel.ColumnCount = 3;
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 200F));
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 200F));
            tableLayoutPanel.Controls.Add(createRoom, 0, 0);
            tableLayoutPanel.Controls.Add(splitContainerDiscoverRoom, 2, 0);
            tableLayoutPanel.Dock = DockStyle.Fill;
            tableLayoutPanel.Location = new Point(0, 0);
            tableLayoutPanel.Margin = new Padding(0);
            tableLayoutPanel.Name = "tableLayoutPanel";
            tableLayoutPanel.RowCount = 1;
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
            tableLayoutPanel.Size = new Size(836, 48);
            tableLayoutPanel.TabIndex = 2;
            // 
            // createRoom
            // 
            createRoom.BackColor = Color.FromArgb(192, 192, 255);
            createRoom.Dock = DockStyle.Fill;
            createRoom.FlatAppearance.BorderColor = Color.FromArgb(228, 230, 240);
            createRoom.FlatStyle = FlatStyle.Flat;
            createRoom.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 204);
            createRoom.ForeColor = Color.FromArgb(248, 249, 255);
            createRoom.Location = new Point(3, 3);
            createRoom.Margin = new Padding(3, 3, 3, 5);
            createRoom.Name = "createRoom";
            createRoom.Size = new Size(194, 42);
            createRoom.TabIndex = 9;
            createRoom.Text = "Создать комнату";
            createRoom.UseVisualStyleBackColor = false;
            createRoom.Click += createRoom_Click;
            // 
            // splitContainerDiscoverRoom
            // 
            splitContainerDiscoverRoom.Dock = DockStyle.Fill;
            splitContainerDiscoverRoom.Location = new Point(639, 3);
            splitContainerDiscoverRoom.Name = "splitContainerDiscoverRoom";
            splitContainerDiscoverRoom.Orientation = Orientation.Horizontal;
            // 
            // splitContainerDiscoverRoom.Panel1
            // 
            splitContainerDiscoverRoom.Panel1.Controls.Add(discoverRooms);
            splitContainerDiscoverRoom.Panel1.Padding = new Padding(0, 0, 0, 2);
            splitContainerDiscoverRoom.Panel1MinSize = 34;
            // 
            // splitContainerDiscoverRoom.Panel2
            // 
            splitContainerDiscoverRoom.Panel2.Controls.Add(discoveryProgressBar);
            splitContainerDiscoverRoom.Panel2Collapsed = true;
            splitContainerDiscoverRoom.Panel2MinSize = 4;
            splitContainerDiscoverRoom.Size = new Size(194, 44);
            splitContainerDiscoverRoom.SplitterDistance = 36;
            splitContainerDiscoverRoom.TabIndex = 5;
            // 
            // discoverRooms
            // 
            discoverRooms.BackColor = Color.FromArgb(248, 249, 255);
            discoverRooms.Dock = DockStyle.Fill;
            discoverRooms.FlatAppearance.BorderColor = Color.FromArgb(167, 157, 255);
            discoverRooms.FlatAppearance.BorderSize = 2;
            discoverRooms.FlatStyle = FlatStyle.Flat;
            discoverRooms.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            discoverRooms.ForeColor = Color.FromArgb(167, 157, 255);
            discoverRooms.Location = new Point(0, 0);
            discoverRooms.Name = "discoverRooms";
            discoverRooms.Size = new Size(194, 42);
            discoverRooms.TabIndex = 0;
            discoverRooms.Text = "Найти комнаты";
            discoverRooms.UseVisualStyleBackColor = false;
            discoverRooms.Click += discoverRooms_Click;
            // 
            // discoveryProgressBar
            // 
            discoveryProgressBar.Dock = DockStyle.Bottom;
            discoveryProgressBar.Location = new Point(0, 17);
            discoveryProgressBar.Margin = new Padding(0);
            discoveryProgressBar.Name = "discoveryProgressBar";
            discoveryProgressBar.Size = new Size(194, 8);
            discoveryProgressBar.TabIndex = 0;
            // 
            // flowLayoutPanelDiscoveredRooms
            // 
            flowLayoutPanelDiscoveredRooms.AutoScroll = true;
            flowLayoutPanelDiscoveredRooms.BackColor = Color.Transparent;
            flowLayoutPanelDiscoveredRooms.Dock = DockStyle.Fill;
            flowLayoutPanelDiscoveredRooms.Location = new Point(0, 0);
            flowLayoutPanelDiscoveredRooms.Margin = new Padding(20);
            flowLayoutPanelDiscoveredRooms.Name = "flowLayoutPanelDiscoveredRooms";
            flowLayoutPanelDiscoveredRooms.Size = new Size(836, 541);
            flowLayoutPanelDiscoveredRooms.TabIndex = 1;
            // 
            // statusStrip
            // 
            statusStrip.ImageScalingSize = new Size(20, 20);
            statusStrip.Items.AddRange(new ToolStripItem[] { totalRoomsCount });
            statusStrip.Location = new Point(0, 519);
            statusStrip.Name = "statusStrip";
            statusStrip.Size = new Size(836, 22);
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
            Size = new Size(836, 591);
            Controls.SetChildIndex(splitContainer, 0);
            splitContainer.Panel1.ResumeLayout(false);
            splitContainer.Panel2.ResumeLayout(false);
            splitContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer).EndInit();
            splitContainer.ResumeLayout(false);
            tableLayoutPanel.ResumeLayout(false);
            splitContainerDiscoverRoom.Panel1.ResumeLayout(false);
            splitContainerDiscoverRoom.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerDiscoverRoom).EndInit();
            splitContainerDiscoverRoom.ResumeLayout(false);
            statusStrip.ResumeLayout(false);
            statusStrip.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanel;
        private SplitContainer splitContainerDiscoverRoom;
        private ProgressBar discoveryProgressBar;
        private FlowLayoutPanel flowLayoutPanelDiscoveredRooms;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel totalRoomsCount;
        private Desktop.Components.Controls.Buttons.InvertedButton discoverRooms;
        private Desktop.Components.Controls.Buttons.CommonButton createRoom;
    }
}
