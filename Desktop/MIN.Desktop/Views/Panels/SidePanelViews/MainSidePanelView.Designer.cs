namespace MIN.Desktop.Views.Panels.SidePanelViews
{
    partial class MainSidePanelView
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
            roomSearchTextBox = new MIN.Desktop.Components.Controls.TextBoxes.DefaultTextBox();
            commonButton2 = new MIN.Desktop.Components.Controls.Buttons.CommonButton();
            commonButton1 = new MIN.Desktop.Components.Controls.Buttons.CommonButton();
            settingsButton = new MIN.Desktop.Components.Controls.Buttons.CommonButton();
            createRoom = new MIN.Desktop.Components.Controls.Buttons.CommonButton();
            tableLayoutPanelHeader = new TableLayoutPanel();
            flowLayoutPanelRooms = new FlowLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)splitContainer).BeginInit();
            splitContainer.Panel1.SuspendLayout();
            splitContainer.Panel2.SuspendLayout();
            splitContainer.SuspendLayout();
            tableLayoutPanelHeader.SuspendLayout();
            SuspendLayout();
            // 
            // splitContainer
            // 
            // 
            // splitContainer.Panel1
            // 
            splitContainer.Panel1.Controls.Add(tableLayoutPanelHeader);
            splitContainer.Panel1MinSize = 88;
            // 
            // splitContainer.Panel2
            // 
            splitContainer.Panel2.Controls.Add(flowLayoutPanelRooms);
            splitContainer.Size = new Size(394, 768);
            splitContainer.SplitterDistance = 88;
            // 
            // roomSearchTextBox
            // 
            roomSearchTextBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            roomSearchTextBox.BackColor = Color.FromArgb(248, 249, 255);
            roomSearchTextBox.BorderStyle = BorderStyle.None;
            roomSearchTextBox.Font = new Font("Segoe UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            roomSearchTextBox.ForeColor = Color.FromArgb(122, 119, 143);
            roomSearchTextBox.Location = new Point(51, 11);
            roomSearchTextBox.Name = "roomSearchTextBox";
            roomSearchTextBox.PlaceholderText = "Поиск";
            roomSearchTextBox.Size = new Size(244, 26);
            roomSearchTextBox.TabIndex = 6;
            // 
            // commonButton2
            // 
            commonButton2.BackColor = Color.FromArgb(167, 157, 255);
            commonButton2.BackgroundImage = Properties.Resources.search;
            commonButton2.BackgroundImageLayout = ImageLayout.Zoom;
            commonButton2.FlatAppearance.BorderColor = Color.FromArgb(228, 230, 240);
            commonButton2.FlatStyle = FlatStyle.Flat;
            commonButton2.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            commonButton2.ForeColor = Color.FromArgb(248, 249, 255);
            commonButton2.Location = new Point(301, 3);
            commonButton2.Name = "commonButton2";
            commonButton2.Size = new Size(42, 42);
            commonButton2.TabIndex = 5;
            commonButton2.UseVisualStyleBackColor = false;
            // 
            // commonButton1
            // 
            commonButton1.BackColor = Color.FromArgb(167, 157, 255);
            commonButton1.BackgroundImage = Properties.Resources.compass;
            commonButton1.BackgroundImageLayout = ImageLayout.Zoom;
            commonButton1.FlatAppearance.BorderColor = Color.FromArgb(228, 230, 240);
            commonButton1.FlatStyle = FlatStyle.Flat;
            commonButton1.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            commonButton1.ForeColor = Color.FromArgb(248, 249, 255);
            commonButton1.Location = new Point(349, 3);
            commonButton1.Name = "commonButton1";
            commonButton1.Size = new Size(42, 42);
            commonButton1.TabIndex = 4;
            commonButton1.UseVisualStyleBackColor = false;
            // 
            // settingsButton
            // 
            settingsButton.BackColor = Color.FromArgb(167, 157, 255);
            settingsButton.BackgroundImage = Properties.Resources.settings;
            settingsButton.BackgroundImageLayout = ImageLayout.Zoom;
            settingsButton.FlatAppearance.BorderColor = Color.FromArgb(228, 230, 240);
            settingsButton.FlatStyle = FlatStyle.Flat;
            settingsButton.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            settingsButton.ForeColor = Color.FromArgb(248, 249, 255);
            settingsButton.Location = new Point(3, 3);
            settingsButton.Name = "settingsButton";
            settingsButton.Size = new Size(42, 42);
            settingsButton.TabIndex = 3;
            settingsButton.UseVisualStyleBackColor = false;
            settingsButton.Click += settingsButton_Click;
            // 
            // createRoom
            // 
            createRoom.BackColor = Color.FromArgb(192, 192, 255);
            tableLayoutPanelHeader.SetColumnSpan(createRoom, 4);
            createRoom.Dock = DockStyle.Fill;
            createRoom.FlatAppearance.BorderColor = Color.FromArgb(228, 230, 240);
            createRoom.FlatStyle = FlatStyle.Flat;
            createRoom.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 204);
            createRoom.ForeColor = Color.FromArgb(248, 249, 255);
            createRoom.Location = new Point(3, 51);
            createRoom.Name = "createRoom";
            createRoom.Size = new Size(388, 34);
            createRoom.TabIndex = 8;
            createRoom.Text = "Создать комнату";
            createRoom.UseVisualStyleBackColor = false;
            createRoom.Click += createRoom_Click;
            // 
            // tableLayoutPanelHeader
            // 
            tableLayoutPanelHeader.ColumnCount = 4;
            tableLayoutPanelHeader.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 48F));
            tableLayoutPanelHeader.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanelHeader.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 48F));
            tableLayoutPanelHeader.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 48F));
            tableLayoutPanelHeader.Controls.Add(createRoom, 0, 1);
            tableLayoutPanelHeader.Controls.Add(settingsButton, 0, 0);
            tableLayoutPanelHeader.Controls.Add(commonButton1, 3, 0);
            tableLayoutPanelHeader.Controls.Add(commonButton2, 2, 0);
            tableLayoutPanelHeader.Controls.Add(roomSearchTextBox, 1, 0);
            tableLayoutPanelHeader.Dock = DockStyle.Fill;
            tableLayoutPanelHeader.Location = new Point(0, 0);
            tableLayoutPanelHeader.Margin = new Padding(0);
            tableLayoutPanelHeader.Name = "tableLayoutPanelHeader";
            tableLayoutPanelHeader.RowCount = 2;
            tableLayoutPanelHeader.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
            tableLayoutPanelHeader.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanelHeader.Size = new Size(394, 88);
            tableLayoutPanelHeader.TabIndex = 1;
            // 
            // flowLayoutPanelRooms
            // 
            flowLayoutPanelRooms.AutoScroll = true;
            flowLayoutPanelRooms.Dock = DockStyle.Fill;
            flowLayoutPanelRooms.Location = new Point(0, 0);
            flowLayoutPanelRooms.Margin = new Padding(20);
            flowLayoutPanelRooms.Name = "flowLayoutPanelRooms";
            flowLayoutPanelRooms.Size = new Size(394, 678);
            flowLayoutPanelRooms.TabIndex = 2;
            // 
            // MainSidePanelView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Name = "MainSidePanelView";
            Size = new Size(394, 768);
            Controls.SetChildIndex(splitContainer, 0);
            splitContainer.Panel1.ResumeLayout(false);
            splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer).EndInit();
            splitContainer.ResumeLayout(false);
            tableLayoutPanelHeader.ResumeLayout(false);
            tableLayoutPanelHeader.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanelHeader;
        private Desktop.Components.Controls.Buttons.CommonButton createRoom;
        private Desktop.Components.Controls.Buttons.CommonButton settingsButton;
        private Desktop.Components.Controls.Buttons.CommonButton commonButton1;
        private Desktop.Components.Controls.Buttons.CommonButton commonButton2;
        private Desktop.Components.Controls.TextBoxes.DefaultTextBox roomSearchTextBox;
        private FlowLayoutPanel flowLayoutPanelRooms;
    }
}
