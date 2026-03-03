namespace MIN.Desktop.Views.Forms.HelperForms
{
    partial class NotificationForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            tableLayoutPanel = new TableLayoutPanel();
            notificationTurnOff = new MIN.Desktop.Components.Labels.CaptionLabel();
            logoName = new MIN.Desktop.Components.Labels.CaptionLabel();
            roomName = new MIN.Desktop.Components.Labels.PrimaryLabel();
            senderAndContent = new MIN.Desktop.Components.Labels.PrimaryLabel();
            logo = new PictureBox();
            closeButton = new MIN.Desktop.Components.Labels.CaptionLabel();
            tableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)logo).BeginInit();
            SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            tableLayoutPanel.ColumnCount = 3;
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 96F));
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 48F));
            tableLayoutPanel.Controls.Add(notificationTurnOff, 1, 0);
            tableLayoutPanel.Controls.Add(logoName, 0, 0);
            tableLayoutPanel.Controls.Add(roomName, 0, 1);
            tableLayoutPanel.Controls.Add(senderAndContent, 0, 2);
            tableLayoutPanel.Controls.Add(logo, 2, 1);
            tableLayoutPanel.Controls.Add(closeButton, 2, 0);
            tableLayoutPanel.Cursor = Cursors.Hand;
            tableLayoutPanel.Dock = DockStyle.Fill;
            tableLayoutPanel.Location = new Point(0, 0);
            tableLayoutPanel.Name = "tableLayoutPanel";
            tableLayoutPanel.RowCount = 3;
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 31.5789471F));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 31.5789471F));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 36.8421059F));
            tableLayoutPanel.Size = new Size(281, 77);
            tableLayoutPanel.TabIndex = 0;
            // 
            // notificationTurnOff
            // 
            notificationTurnOff.Anchor = AnchorStyles.Right;
            notificationTurnOff.AutoSize = true;
            notificationTurnOff.Cursor = Cursors.Hand;
            notificationTurnOff.Font = new Font("Segoe UI", 8F);
            notificationTurnOff.ForeColor = Color.FromArgb(64, 64, 64);
            notificationTurnOff.Location = new Point(162, 5);
            notificationTurnOff.Name = "notificationTurnOff";
            notificationTurnOff.Size = new Size(68, 13);
            notificationTurnOff.TabIndex = 5;
            notificationTurnOff.Text = "Отписаться";
            notificationTurnOff.Click += notificationTurnOff_Click;
            // 
            // logoName
            // 
            logoName.AutoSize = true;
            logoName.Font = new Font("Segoe UI", 8F);
            logoName.ForeColor = Color.FromArgb(64, 64, 64);
            logoName.Location = new Point(3, 0);
            logoName.Name = "logoName";
            logoName.Size = new Size(28, 13);
            logoName.TabIndex = 1;
            logoName.Text = "MIN";
            // 
            // roomName
            // 
            roomName.AutoEllipsis = true;
            roomName.AutoSize = true;
            tableLayoutPanel.SetColumnSpan(roomName, 2);
            roomName.Cursor = Cursors.Hand;
            roomName.Dock = DockStyle.Fill;
            roomName.Font = new Font("Segoe UI", 9.75F);
            roomName.ForeColor = Color.FromArgb(45, 43, 58);
            roomName.Location = new Point(3, 24);
            roomName.Name = "roomName";
            roomName.Size = new Size(227, 24);
            roomName.TabIndex = 2;
            roomName.Text = "Загрузка...";
            // 
            // senderAndContent
            // 
            senderAndContent.AutoEllipsis = true;
            senderAndContent.AutoSize = true;
            tableLayoutPanel.SetColumnSpan(senderAndContent, 2);
            senderAndContent.Cursor = Cursors.Hand;
            senderAndContent.Dock = DockStyle.Fill;
            senderAndContent.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            senderAndContent.ForeColor = Color.FromArgb(45, 43, 58);
            senderAndContent.Location = new Point(3, 48);
            senderAndContent.Name = "senderAndContent";
            senderAndContent.Size = new Size(227, 29);
            senderAndContent.TabIndex = 3;
            senderAndContent.Text = "Загрузка...";
            // 
            // logo
            // 
            logo.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            logo.BackgroundImage = Properties.Resources.logoImage;
            logo.BackgroundImageLayout = ImageLayout.Zoom;
            logo.Location = new Point(236, 27);
            logo.Name = "logo";
            tableLayoutPanel.SetRowSpan(logo, 2);
            logo.Size = new Size(42, 47);
            logo.TabIndex = 0;
            logo.TabStop = false;
            // 
            // closeButton
            // 
            closeButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            closeButton.AutoSize = true;
            closeButton.BorderStyle = BorderStyle.FixedSingle;
            closeButton.Cursor = Cursors.Hand;
            closeButton.FlatStyle = FlatStyle.Flat;
            closeButton.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 204);
            closeButton.ForeColor = Color.FromArgb(64, 64, 64);
            closeButton.Location = new Point(260, 3);
            closeButton.Margin = new Padding(3);
            closeButton.Name = "closeButton";
            closeButton.Size = new Size(18, 18);
            closeButton.TabIndex = 4;
            closeButton.Text = "X";
            closeButton.Click += closeButton_Click;
            // 
            // NotificationForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(281, 77);
            Controls.Add(tableLayoutPanel);
            FormBorderStyle = FormBorderStyle.None;
            Name = "NotificationForm";
            Text = "NotificationForm";
            tableLayoutPanel.ResumeLayout(false);
            tableLayoutPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)logo).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanel;
        private PictureBox logo;
        private Desktop.Components.Labels.CaptionLabel logoName;
        private Desktop.Components.Labels.PrimaryLabel roomName;
        private Desktop.Components.Labels.PrimaryLabel senderAndContent;
        private Desktop.Components.Labels.CaptionLabel closeButton;
        private Desktop.Components.Labels.CaptionLabel notificationTurnOff;
    }
}