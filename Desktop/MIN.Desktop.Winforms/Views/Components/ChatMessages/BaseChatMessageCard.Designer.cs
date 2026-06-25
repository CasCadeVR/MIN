namespace MIN.Desktop.Views.Components.ChatMessages
{
    partial class BaseChatMessageCard
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
            TableLayoutPanel = new TableLayoutPanel();
            sendTime = new MIN.Desktop.Components.Textboxes.ReadonlyTextbox();
            sendRole = new MIN.Desktop.Components.Textboxes.ReadonlyTextbox();
            senderName = new MIN.Desktop.Components.Textboxes.ReadonlyTextbox();
            TableLayoutPanel.SuspendLayout();
            SuspendLayout();
            // 
            // TableLayoutPanel
            // 
            TableLayoutPanel.ColumnCount = 2;
            TableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            TableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 32F));
            TableLayoutPanel.Controls.Add(sendTime, 1, 1);
            TableLayoutPanel.Controls.Add(sendRole, 1, 0);
            TableLayoutPanel.Controls.Add(senderName, 0, 0);
            TableLayoutPanel.Dock = DockStyle.Fill;
            TableLayoutPanel.Location = new Point(0, 0);
            TableLayoutPanel.Margin = new Padding(0);
            TableLayoutPanel.Name = "TableLayoutPanel";
            TableLayoutPanel.RowCount = 2;
            TableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 22F));
            TableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            TableLayoutPanel.Size = new Size(311, 70);
            TableLayoutPanel.TabIndex = 3;
            // 
            // sendTime
            // 
            sendTime.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            sendTime.BackColor = SystemColors.Control;
            sendTime.BorderStyle = BorderStyle.None;
            sendTime.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 204);
            sendTime.ForeColor = Color.FromArgb(0, 0, 0);
            sendTime.Location = new Point(279, 55);
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
            sendRole.Location = new Point(282, 2);
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
            senderName.Size = new Size(276, 18);
            senderName.TabIndex = 0;
            senderName.Text = "Отправитель";
            senderName.WordWrap = false;
            // 
            // BaseChatMessageCard
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(TableLayoutPanel);
            MinimumSize = new Size(0, 22);
            Name = "BaseChatMessageCard";
            Size = new Size(311, 70);
            TableLayoutPanel.ResumeLayout(false);
            TableLayoutPanel.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        /// <summary>
        /// <inheritdoc cref="TableLayoutPanel"/>
        /// </summary>
        public TableLayoutPanel TableLayoutPanel;

        /// <summary>
        /// <inheritdoc  cref="Desktop.Components.Textboxes.ReadonlyTextbox"/>
        /// </summary>
        protected Desktop.Components.Textboxes.ReadonlyTextbox sendTime;

        /// <summary>
        /// <inheritdoc  cref="Desktop.Components.Textboxes.ReadonlyTextbox"/>
        /// </summary>
        protected Desktop.Components.Textboxes.ReadonlyTextbox senderName;

        /// <summary>
        /// <inheritdoc  cref="Desktop.Components.Textboxes.ReadonlyTextbox"/>
        /// </summary>
        protected Desktop.Components.Textboxes.ReadonlyTextbox sendRole;
    }
}
