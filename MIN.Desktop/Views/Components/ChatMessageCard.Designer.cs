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
            sendTime = new MIN.Desktop.Components.Textboxes.ReadonlyTextbox();
            sendRole = new MIN.Desktop.Components.Textboxes.ReadonlyTextbox();
            senderName = new MIN.Desktop.Components.Textboxes.ReadonlyTextbox();
            sendMessage = new MIN.Desktop.Components.Textboxes.ReadonlyTextbox();
            tableLayoutPanelLabels.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanelLabels
            // 
            tableLayoutPanelLabels.ColumnCount = 2;
            tableLayoutPanelLabels.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanelLabels.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 73F));
            tableLayoutPanelLabels.Controls.Add(sendTime, 1, 1);
            tableLayoutPanelLabels.Controls.Add(sendRole, 1, 0);
            tableLayoutPanelLabels.Controls.Add(senderName, 0, 0);
            tableLayoutPanelLabels.Controls.Add(sendMessage, 0, 1);
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
            sendTime.BackColor = Color.FromArgb(248, 249, 255);
            sendTime.BorderStyle = BorderStyle.None;
            sendTime.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            sendTime.ForeColor = Color.FromArgb(0, 0, 0);
            sendTime.Location = new Point(260, 49);
            sendTime.Name = "sendTime";
            sendTime.ReadOnly = true;
            sendTime.Size = new Size(48, 18);
            sendTime.TabIndex = 4;
            sendTime.Text = "Время";
            sendTime.TextAlign = HorizontalAlignment.Right;
            // 
            // sendRole
            // 
            sendRole.BackColor = Color.FromArgb(248, 249, 255);
            sendRole.BorderStyle = BorderStyle.None;
            sendRole.Dock = DockStyle.Right;
            sendRole.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            sendRole.ForeColor = Color.FromArgb(0, 0, 0);
            sendRole.Location = new Point(269, 3);
            sendRole.Name = "sendRole";
            sendRole.ReadOnly = true;
            sendRole.Size = new Size(39, 18);
            sendRole.TabIndex = 1;
            sendRole.Text = "Роль";
            sendRole.TextAlign = HorizontalAlignment.Right;
            // 
            // senderName
            // 
            senderName.BackColor = Color.FromArgb(248, 249, 255);
            senderName.BorderStyle = BorderStyle.None;
            senderName.Dock = DockStyle.Fill;
            senderName.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            senderName.ForeColor = Color.FromArgb(0, 0, 0);
            senderName.Location = new Point(3, 3);
            senderName.Name = "senderName";
            senderName.ReadOnly = true;
            senderName.Size = new Size(232, 18);
            senderName.TabIndex = 0;
            senderName.Text = "Отправитель";
            // 
            // sendMessage
            // 
            sendMessage.BackColor = SystemColors.Control;
            sendMessage.BorderStyle = BorderStyle.None;
            sendMessage.Dock = DockStyle.Fill;
            sendMessage.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            sendMessage.ForeColor = Color.Black;
            sendMessage.Location = new Point(0, 24);
            sendMessage.Margin = new Padding(0);
            sendMessage.Multiline = true;
            sendMessage.Name = "sendMessage";
            sendMessage.ReadOnly = true;
            sendMessage.Size = new Size(238, 46);
            sendMessage.TabIndex = 5;
            sendMessage.Text = "Сообщение";
            // 
            // ChatMessageCard
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(tableLayoutPanelLabels);
            MinimumSize = new Size(0, 60);
            Name = "ChatMessageCard";
            Size = new Size(311, 70);
            tableLayoutPanelLabels.ResumeLayout(false);
            tableLayoutPanelLabels.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanelLabels;
        private Textboxes.ReadonlyTextbox sendTime;
        private Textboxes.ReadonlyTextbox sendRole;
        private Textboxes.ReadonlyTextbox senderName;
        private Textboxes.ReadonlyTextbox sendMessage;
    }
}
