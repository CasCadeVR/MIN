namespace MIN.Desktop.Components
{
    partial class ChatFileMessageCard
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
            fileInterractButton = new MIN.Desktop.Components.Controls.Buttons.CommonButton();
            fileName = new MIN.Desktop.Components.Labels.Heading3Label();
            splitContainerDownload = new SplitContainer();
            fileSize = new MIN.Desktop.Components.Labels.Heading3Label();
            downloadProgressBar = new ProgressBar();
            tableLayoutPanel = new TableLayoutPanel();
            senderName = new MIN.Desktop.Components.Textboxes.ReadonlyTextbox();
            sendRole = new MIN.Desktop.Components.Textboxes.ReadonlyTextbox();
            sendTime = new MIN.Desktop.Components.Textboxes.ReadonlyTextbox();
            tableLayoutPanelLabels.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainerDownload).BeginInit();
            splitContainerDownload.Panel1.SuspendLayout();
            splitContainerDownload.Panel2.SuspendLayout();
            splitContainerDownload.SuspendLayout();
            tableLayoutPanel.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanelLabels
            // 
            tableLayoutPanelLabels.ColumnCount = 2;
            tableLayoutPanelLabels.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 70F));
            tableLayoutPanelLabels.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanelLabels.Controls.Add(fileInterractButton, 0, 0);
            tableLayoutPanelLabels.Controls.Add(fileName, 1, 0);
            tableLayoutPanelLabels.Controls.Add(splitContainerDownload, 1, 1);
            tableLayoutPanelLabels.Dock = DockStyle.Fill;
            tableLayoutPanelLabels.Location = new Point(0, 22);
            tableLayoutPanelLabels.Margin = new Padding(0);
            tableLayoutPanelLabels.Name = "tableLayoutPanelLabels";
            tableLayoutPanelLabels.RowCount = 2;
            tableLayoutPanelLabels.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanelLabels.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanelLabels.Size = new Size(218, 73);
            tableLayoutPanelLabels.TabIndex = 3;
            // 
            // fileInterractButton
            // 
            fileInterractButton.BackColor = Color.FromArgb(167, 157, 255);
            fileInterractButton.BackgroundImageLayout = ImageLayout.Zoom;
            fileInterractButton.Dock = DockStyle.Fill;
            fileInterractButton.FlatAppearance.BorderColor = Color.FromArgb(228, 230, 240);
            fileInterractButton.FlatStyle = FlatStyle.Flat;
            fileInterractButton.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            fileInterractButton.ForeColor = Color.FromArgb(248, 249, 255);
            fileInterractButton.Location = new Point(3, 3);
            fileInterractButton.Name = "fileInterractButton";
            tableLayoutPanelLabels.SetRowSpan(fileInterractButton, 2);
            fileInterractButton.Size = new Size(64, 67);
            fileInterractButton.TabIndex = 4;
            fileInterractButton.Text = "Тип файла";
            fileInterractButton.UseVisualStyleBackColor = false;
            fileInterractButton.Click += fileInterractButton_Click;
            fileInterractButton.MouseLeave += fileInterractButton_MouseLeave;
            fileInterractButton.MouseHover += fileInterractButton_MouseHover;
            // 
            // fileName
            // 
            fileName.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            fileName.AutoEllipsis = true;
            fileName.AutoSize = true;
            fileName.BackColor = Color.Transparent;
            fileName.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 204);
            fileName.ForeColor = Color.FromArgb(0, 0, 0);
            fileName.Location = new Point(73, 16);
            fileName.MaximumSize = new Size(0, 20);
            fileName.Name = "fileName";
            fileName.Size = new Size(142, 20);
            fileName.TabIndex = 3;
            fileName.Text = "Имя файла";
            fileName.TextAlign = ContentAlignment.BottomLeft;
            // 
            // splitContainerDownload
            // 
            splitContainerDownload.Location = new Point(70, 36);
            splitContainerDownload.Margin = new Padding(0);
            splitContainerDownload.Name = "splitContainerDownload";
            splitContainerDownload.Orientation = Orientation.Horizontal;
            // 
            // splitContainerDownload.Panel1
            // 
            splitContainerDownload.Panel1.Controls.Add(fileSize);
            // 
            // splitContainerDownload.Panel2
            // 
            splitContainerDownload.Panel2.Controls.Add(downloadProgressBar);
            splitContainerDownload.Panel2Collapsed = true;
            splitContainerDownload.Size = new Size(148, 37);
            splitContainerDownload.SplitterDistance = 25;
            splitContainerDownload.TabIndex = 5;
            // 
            // fileSize
            // 
            fileSize.AutoEllipsis = true;
            fileSize.AutoSize = true;
            fileSize.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 204);
            fileSize.ForeColor = Color.FromArgb(0, 0, 0);
            fileSize.Location = new Point(0, 0);
            fileSize.Name = "fileSize";
            fileSize.Size = new Size(85, 13);
            fileSize.TabIndex = 1;
            fileSize.Text = "Размер файла";
            // 
            // downloadProgressBar
            // 
            downloadProgressBar.Dock = DockStyle.Top;
            downloadProgressBar.Location = new Point(0, 0);
            downloadProgressBar.Margin = new Padding(0);
            downloadProgressBar.Name = "downloadProgressBar";
            downloadProgressBar.Size = new Size(148, 8);
            downloadProgressBar.Step = 1;
            downloadProgressBar.TabIndex = 1;
            // 
            // tableLayoutPanel
            // 
            tableLayoutPanel.ColumnCount = 2;
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 32F));
            tableLayoutPanel.Controls.Add(tableLayoutPanelLabels, 0, 1);
            tableLayoutPanel.Controls.Add(senderName, 0, 0);
            tableLayoutPanel.Controls.Add(sendRole, 1, 0);
            tableLayoutPanel.Controls.Add(sendTime, 1, 1);
            tableLayoutPanel.Dock = DockStyle.Fill;
            tableLayoutPanel.Location = new Point(0, 0);
            tableLayoutPanel.Margin = new Padding(0);
            tableLayoutPanel.Name = "tableLayoutPanel";
            tableLayoutPanel.RowCount = 2;
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 22F));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 70F));
            tableLayoutPanel.Size = new Size(250, 95);
            tableLayoutPanel.TabIndex = 4;
            // 
            // senderName
            // 
            senderName.BackColor = Color.FromArgb(248, 249, 255);
            senderName.BorderStyle = BorderStyle.None;
            senderName.Dock = DockStyle.Fill;
            senderName.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            senderName.ForeColor = Color.FromArgb(0, 0, 0);
            senderName.Location = new Point(3, 3);
            senderName.Margin = new Padding(3, 3, 0, 0);
            senderName.Name = "senderName";
            senderName.ReadOnly = true;
            senderName.Size = new Size(215, 18);
            senderName.TabIndex = 0;
            senderName.Text = "Отправитель";
            senderName.WordWrap = false;
            // 
            // sendRole
            // 
            sendRole.BackColor = Color.FromArgb(248, 249, 255);
            sendRole.BorderStyle = BorderStyle.None;
            sendRole.Dock = DockStyle.Right;
            sendRole.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 204);
            sendRole.ForeColor = Color.FromArgb(0, 0, 0);
            sendRole.Location = new Point(221, 2);
            sendRole.Margin = new Padding(3, 2, 3, 2);
            sendRole.Name = "sendRole";
            sendRole.ReadOnly = true;
            sendRole.Size = new Size(26, 15);
            sendRole.TabIndex = 1;
            sendRole.Text = "Роль";
            sendRole.TextAlign = HorizontalAlignment.Right;
            // 
            // sendTime
            // 
            sendTime.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            sendTime.BackColor = Color.FromArgb(248, 249, 255);
            sendTime.BorderStyle = BorderStyle.None;
            sendTime.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 204);
            sendTime.ForeColor = Color.FromArgb(0, 0, 0);
            sendTime.Location = new Point(218, 80);
            sendTime.Margin = new Padding(0, 0, 3, 0);
            sendTime.Name = "sendTime";
            sendTime.ReadOnly = true;
            sendTime.Size = new Size(29, 15);
            sendTime.TabIndex = 4;
            sendTime.Text = "Время";
            sendTime.TextAlign = HorizontalAlignment.Right;
            // 
            // ChatFileMessageCard
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(tableLayoutPanel);
            MaximumSize = new Size(250, 95);
            MinimumSize = new Size(179, 22);
            Name = "ChatFileMessageCard";
            Size = new Size(250, 95);
            tableLayoutPanelLabels.ResumeLayout(false);
            tableLayoutPanelLabels.PerformLayout();
            splitContainerDownload.Panel1.ResumeLayout(false);
            splitContainerDownload.Panel1.PerformLayout();
            splitContainerDownload.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerDownload).EndInit();
            splitContainerDownload.ResumeLayout(false);
            tableLayoutPanel.ResumeLayout(false);
            tableLayoutPanel.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Labels.Heading3Label fileSize;
        private TableLayoutPanel tableLayoutPanel;
        private TableLayoutPanel tableLayoutPanelLabels;
        private Textboxes.ReadonlyTextbox sendTime;
        private Textboxes.ReadonlyTextbox sendRole;
        private Textboxes.ReadonlyTextbox senderName;
        private Labels.Heading3Label fileName;
        private Controls.Buttons.CommonButton fileInterractButton;
        private SplitContainer splitContainerDownload;
        private ProgressBar downloadProgressBar;
    }
}
