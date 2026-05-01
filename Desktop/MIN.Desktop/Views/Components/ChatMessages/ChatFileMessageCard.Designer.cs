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
            fileType = new MIN.Desktop.Components.Labels.Heading3Label();
            fileName = new MIN.Desktop.Components.Textboxes.ReadonlyTextbox();
            fileSize = new MIN.Desktop.Components.Labels.Heading3Label();
            tableLayoutPanel = new TableLayoutPanel();
            sendTime = new MIN.Desktop.Components.Textboxes.ReadonlyTextbox();
            sendRole = new MIN.Desktop.Components.Textboxes.ReadonlyTextbox();
            senderName = new MIN.Desktop.Components.Textboxes.ReadonlyTextbox();
            tableLayoutPanelLabels.SuspendLayout();
            tableLayoutPanel.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanelLabels
            // 
            tableLayoutPanelLabels.ColumnCount = 2;
            tableLayoutPanelLabels.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 70F));
            tableLayoutPanelLabels.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanelLabels.Controls.Add(fileType, 0, 0);
            tableLayoutPanelLabels.Controls.Add(fileName, 1, 0);
            tableLayoutPanelLabels.Controls.Add(fileSize, 1, 1);
            tableLayoutPanelLabels.Dock = DockStyle.Fill;
            tableLayoutPanelLabels.Location = new Point(0, 22);
            tableLayoutPanelLabels.Margin = new Padding(0);
            tableLayoutPanelLabels.Name = "tableLayoutPanelLabels";
            tableLayoutPanelLabels.RowCount = 2;
            tableLayoutPanelLabels.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanelLabels.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanelLabels.Size = new Size(299, 70);
            tableLayoutPanelLabels.TabIndex = 3;
            // 
            // fileType
            // 
            fileType.Anchor = AnchorStyles.None;
            fileType.AutoEllipsis = true;
            fileType.AutoSize = true;
            fileType.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 204);
            fileType.ForeColor = Color.FromArgb(0, 0, 0);
            fileType.Location = new Point(3, 28);
            fileType.Name = "fileType";
            tableLayoutPanelLabels.SetRowSpan(fileType, 2);
            fileType.Size = new Size(64, 13);
            fileType.TabIndex = 2;
            fileType.Text = "Тип файла";
            // 
            // fileName
            // 
            fileName.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            fileName.BackColor = SystemColors.Control;
            fileName.BorderStyle = BorderStyle.None;
            fileName.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            fileName.ForeColor = Color.FromArgb(0, 0, 0);
            fileName.Location = new Point(73, 17);
            fileName.Margin = new Padding(3, 3, 0, 0);
            fileName.Name = "fileName";
            fileName.ReadOnly = true;
            fileName.Size = new Size(226, 18);
            fileName.TabIndex = 0;
            fileName.Text = "Имя файла";
            fileName.WordWrap = false;
            // 
            // fileSize
            // 
            fileSize.AutoEllipsis = true;
            fileSize.AutoSize = true;
            fileSize.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 204);
            fileSize.ForeColor = Color.FromArgb(0, 0, 0);
            fileSize.Location = new Point(73, 35);
            fileSize.Name = "fileSize";
            fileSize.Size = new Size(85, 13);
            fileSize.TabIndex = 1;
            fileSize.Text = "Размер файла";
            // 
            // tableLayoutPanel
            // 
            tableLayoutPanel.ColumnCount = 2;
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 32F));
            tableLayoutPanel.Controls.Add(sendTime, 1, 1);
            tableLayoutPanel.Controls.Add(tableLayoutPanelLabels, 0, 1);
            tableLayoutPanel.Controls.Add(sendRole, 1, 0);
            tableLayoutPanel.Controls.Add(senderName, 0, 0);
            tableLayoutPanel.Dock = DockStyle.Fill;
            tableLayoutPanel.Location = new Point(0, 0);
            tableLayoutPanel.Margin = new Padding(0);
            tableLayoutPanel.Name = "tableLayoutPanel";
            tableLayoutPanel.RowCount = 2;
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 22F));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 70F));
            tableLayoutPanel.Size = new Size(331, 92);
            tableLayoutPanel.TabIndex = 4;
            // 
            // sendTime
            // 
            sendTime.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            sendTime.BackColor = SystemColors.Control;
            sendTime.BorderStyle = BorderStyle.None;
            sendTime.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 204);
            sendTime.ForeColor = Color.FromArgb(0, 0, 0);
            sendTime.Location = new Point(299, 77);
            sendTime.Margin = new Padding(0, 0, 3, 0);
            sendTime.Name = "sendTime";
            sendTime.ReadOnly = true;
            sendTime.Size = new Size(29, 15);
            sendTime.TabIndex = 4;
            sendTime.Text = "Время";
            sendTime.TextAlign = HorizontalAlignment.Right;
            // 
            // sendRole
            // 
            sendRole.BackColor = SystemColors.Control;
            sendRole.BorderStyle = BorderStyle.None;
            sendRole.Dock = DockStyle.Right;
            sendRole.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 204);
            sendRole.ForeColor = Color.FromArgb(0, 0, 0);
            sendRole.Location = new Point(302, 2);
            sendRole.Margin = new Padding(3, 2, 3, 2);
            sendRole.Name = "sendRole";
            sendRole.ReadOnly = true;
            sendRole.Size = new Size(26, 15);
            sendRole.TabIndex = 1;
            sendRole.Text = "Роль";
            sendRole.TextAlign = HorizontalAlignment.Right;
            // 
            // senderName
            // 
            senderName.BackColor = SystemColors.Control;
            senderName.BorderStyle = BorderStyle.None;
            senderName.Dock = DockStyle.Fill;
            senderName.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            senderName.ForeColor = Color.FromArgb(0, 0, 0);
            senderName.Location = new Point(3, 3);
            senderName.Margin = new Padding(3, 3, 0, 0);
            senderName.Name = "senderName";
            senderName.ReadOnly = true;
            senderName.Size = new Size(296, 18);
            senderName.TabIndex = 0;
            senderName.Text = "Отправитель";
            senderName.WordWrap = false;
            // 
            // ChatFileMessageCard
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(tableLayoutPanel);
            Margin = new Padding(0);
            MinimumSize = new Size(0, 22);
            Name = "ChatFileMessageCard";
            Size = new Size(331, 92);
            tableLayoutPanelLabels.ResumeLayout(false);
            tableLayoutPanelLabels.PerformLayout();
            tableLayoutPanel.ResumeLayout(false);
            tableLayoutPanel.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanelLabels;
        private Labels.Heading3Label fileType;
        private Textboxes.ReadonlyTextbox fileName;
        private Labels.Heading3Label fileSize;
        private TableLayoutPanel tableLayoutPanel;
        private Textboxes.ReadonlyTextbox sendTime;
        private Textboxes.ReadonlyTextbox sendRole;
        private Textboxes.ReadonlyTextbox senderName;
    }
}
