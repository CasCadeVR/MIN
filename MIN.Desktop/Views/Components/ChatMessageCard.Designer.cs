namespace MIN.Desktop.Components
{
    partial class ChatMessageCard
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
            sendTime = new MIN.Desktop.Components.Labels.Heading3Label();
            sendMessage = new MIN.Desktop.Components.Labels.Heading3Label();
            sendRole = new MIN.Desktop.Components.Labels.Heading3Label();
            senderName = new MIN.Desktop.Components.Labels.Heading3Label();
            tableLayoutPanelLabels.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanelLabels
            // 
            tableLayoutPanelLabels.ColumnCount = 2;
            tableLayoutPanelLabels.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanelLabels.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 64F));
            tableLayoutPanelLabels.Controls.Add(sendTime, 1, 1);
            tableLayoutPanelLabels.Controls.Add(sendMessage, 0, 1);
            tableLayoutPanelLabels.Controls.Add(sendRole, 1, 0);
            tableLayoutPanelLabels.Controls.Add(senderName, 0, 0);
            tableLayoutPanelLabels.Dock = DockStyle.Fill;
            tableLayoutPanelLabels.Location = new Point(0, 0);
            tableLayoutPanelLabels.Margin = new Padding(0);
            tableLayoutPanelLabels.Name = "tableLayoutPanelLabels";
            tableLayoutPanelLabels.RowCount = 2;
            tableLayoutPanelLabels.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));
            tableLayoutPanelLabels.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanelLabels.Size = new Size(311, 70);
            tableLayoutPanelLabels.TabIndex = 2;
            // 
            // sendTime
            // 
            sendTime.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            sendTime.AutoSize = true;
            sendTime.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            sendTime.ForeColor = Color.FromArgb(0, 0, 0);
            sendTime.Location = new Point(260, 53);
            sendTime.Name = "sendTime";
            sendTime.Size = new Size(48, 17);
            sendTime.TabIndex = 4;
            sendTime.Text = "Время";
            // 
            // sendMessage
            // 
            sendMessage.AutoSize = true;
            sendMessage.Dock = DockStyle.Fill;
            sendMessage.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            sendMessage.ForeColor = Color.FromArgb(0, 0, 0);
            sendMessage.Location = new Point(3, 24);
            sendMessage.Name = "sendMessage";
            sendMessage.Size = new Size(241, 46);
            sendMessage.TabIndex = 2;
            sendMessage.Text = "Сообщение";
            // 
            // sendRole
            // 
            sendRole.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            sendRole.AutoSize = true;
            sendRole.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            sendRole.ForeColor = Color.FromArgb(0, 0, 0);
            sendRole.Location = new Point(269, 0);
            sendRole.Name = "sendRole";
            sendRole.Size = new Size(39, 17);
            sendRole.TabIndex = 1;
            sendRole.Text = "Роль";
            // 
            // senderName
            // 
            senderName.AutoSize = true;
            senderName.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            senderName.ForeColor = Color.FromArgb(0, 0, 0);
            senderName.Location = new Point(3, 0);
            senderName.Name = "senderName";
            senderName.Size = new Size(91, 17);
            senderName.TabIndex = 0;
            senderName.Text = "Отправитель";
            // 
            // ChatMessageCard
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(tableLayoutPanelLabels);
            Margin = new Padding(0, 0, 0, 3);
            MaximumSize = new Size(311, 0);
            MinimumSize = new Size(311, 70);
            Name = "ChatMessageCard";
            Size = new Size(311, 70);
            tableLayoutPanelLabels.ResumeLayout(false);
            tableLayoutPanelLabels.PerformLayout();
            ResumeLayout(false);

        }

        #endregion

        private TableLayoutPanel tableLayoutPanelLabels;
        private Labels.Heading3Label sendTime;
        private Labels.Heading3Label sendMessage;
        private Labels.Heading3Label sendRole;
        private Labels.Heading3Label senderName;
    }
}
