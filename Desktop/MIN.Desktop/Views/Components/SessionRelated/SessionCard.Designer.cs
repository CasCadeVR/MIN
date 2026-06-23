namespace MIN.Desktop.Components
{
    partial class SessionCard
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
            sessionVersion = new MIN.Desktop.Components.Labels.PrimaryLabel();
            downloadLinkLabel = new MIN.Desktop.Components.Labels.PrimaryLabel();
            sessionMaximumParticipants = new MIN.Desktop.Components.Labels.PrimaryLabel();
            sessionName = new MIN.Desktop.Components.Labels.Heading3Label();
            sessionDescription = new MIN.Desktop.Components.Labels.PrimaryLabel();
            sessionImage = new MIN.Desktop.Components.Controls.PictureBoxes.DefaultPictureBox();
            tableLayoutPanelLabels.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)sessionImage).BeginInit();
            SuspendLayout();
            // 
            // tableLayoutPanelLabels
            // 
            tableLayoutPanelLabels.BackColor = Color.Transparent;
            tableLayoutPanelLabels.ColumnCount = 1;
            tableLayoutPanelLabels.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanelLabels.Controls.Add(sessionVersion, 0, 5);
            tableLayoutPanelLabels.Controls.Add(downloadLinkLabel, 0, 4);
            tableLayoutPanelLabels.Controls.Add(sessionMaximumParticipants, 0, 3);
            tableLayoutPanelLabels.Controls.Add(sessionName, 0, 1);
            tableLayoutPanelLabels.Controls.Add(sessionDescription, 0, 2);
            tableLayoutPanelLabels.Controls.Add(sessionImage, 0, 0);
            tableLayoutPanelLabels.Dock = DockStyle.Fill;
            tableLayoutPanelLabels.Location = new Point(5, 5);
            tableLayoutPanelLabels.Margin = new Padding(0);
            tableLayoutPanelLabels.Name = "tableLayoutPanelLabels";
            tableLayoutPanelLabels.RowCount = 6;
            tableLayoutPanelLabels.RowStyles.Add(new RowStyle(SizeType.Absolute, 205F));
            tableLayoutPanelLabels.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            tableLayoutPanelLabels.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanelLabels.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));
            tableLayoutPanelLabels.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));
            tableLayoutPanelLabels.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));
            tableLayoutPanelLabels.Size = new Size(240, 370);
            tableLayoutPanelLabels.TabIndex = 2;
            tableLayoutPanelLabels.Click += card_Click;
            // 
            // sessionVersion
            // 
            sessionVersion.Anchor = AnchorStyles.None;
            sessionVersion.AutoSize = true;
            sessionVersion.Font = new Font("Segoe UI", 9.75F);
            sessionVersion.ForeColor = Color.FromArgb(45, 43, 58);
            sessionVersion.Location = new Point(95, 349);
            sessionVersion.Name = "sessionVersion";
            sessionVersion.Size = new Size(50, 17);
            sessionVersion.TabIndex = 5;
            sessionVersion.Text = "Версия";
            sessionVersion.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // downloadLinkLabel
            // 
            downloadLinkLabel.Anchor = AnchorStyles.None;
            downloadLinkLabel.AutoSize = true;
            downloadLinkLabel.Cursor = Cursors.Hand;
            downloadLinkLabel.Font = new Font("Segoe UI", 9.75F);
            downloadLinkLabel.ForeColor = Color.Blue;
            downloadLinkLabel.Location = new Point(15, 325);
            downloadLinkLabel.Name = "downloadLinkLabel";
            downloadLinkLabel.Size = new Size(210, 17);
            downloadLinkLabel.TabIndex = 4;
            downloadLinkLabel.Text = "Скопировать ссылку на установку";
            downloadLinkLabel.TextAlign = ContentAlignment.MiddleCenter;
            downloadLinkLabel.Click += downloadLinkLabel_Click;
            // 
            // sessionMaximumParticipants
            // 
            sessionMaximumParticipants.Anchor = AnchorStyles.None;
            sessionMaximumParticipants.AutoSize = true;
            sessionMaximumParticipants.Font = new Font("Segoe UI", 9.75F);
            sessionMaximumParticipants.ForeColor = Color.FromArgb(45, 43, 58);
            sessionMaximumParticipants.Location = new Point(46, 301);
            sessionMaximumParticipants.Name = "sessionMaximumParticipants";
            sessionMaximumParticipants.Size = new Size(147, 17);
            sessionMaximumParticipants.TabIndex = 3;
            sessionMaximumParticipants.Text = "Максимум участников: ";
            sessionMaximumParticipants.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // sessionName
            // 
            sessionName.Anchor = AnchorStyles.None;
            sessionName.AutoSize = true;
            sessionName.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            sessionName.ForeColor = Color.FromArgb(45, 43, 58);
            sessionName.Location = new Point(77, 210);
            sessionName.Name = "sessionName";
            sessionName.Size = new Size(86, 21);
            sessionName.TabIndex = 0;
            sessionName.Text = "Название";
            sessionName.Click += card_Click;
            // 
            // sessionDescription
            // 
            sessionDescription.Anchor = AnchorStyles.None;
            sessionDescription.AutoSize = true;
            sessionDescription.Font = new Font("Segoe UI", 9.75F);
            sessionDescription.ForeColor = Color.FromArgb(45, 43, 58);
            sessionDescription.Location = new Point(87, 259);
            sessionDescription.Name = "sessionDescription";
            sessionDescription.Size = new Size(66, 17);
            sessionDescription.TabIndex = 1;
            sessionDescription.Text = "Описание";
            sessionDescription.TextAlign = ContentAlignment.MiddleCenter;
            sessionDescription.Click += card_Click;
            // 
            // sessionImage
            // 
            sessionImage.BackColor = Color.Gray;
            sessionImage.Dock = DockStyle.Fill;
            sessionImage.Location = new Point(3, 3);
            sessionImage.Name = "sessionImage";
            sessionImage.Size = new Size(234, 199);
            sessionImage.SizeMode = PictureBoxSizeMode.StretchImage;
            sessionImage.TabIndex = 2;
            sessionImage.TabStop = false;
            sessionImage.Click += card_Click;
            // 
            // SessionCard
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(tableLayoutPanelLabels);
            Name = "SessionCard";
            Padding = new Padding(5);
            Size = new Size(250, 380);
            tableLayoutPanelLabels.ResumeLayout(false);
            tableLayoutPanelLabels.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)sessionImage).EndInit();
            ResumeLayout(false);
        }

        #endregion
        private TableLayoutPanel tableLayoutPanelLabels;
        private Labels.Heading3Label sessionName;
        private Labels.PrimaryLabel sessionDescription;
        private Controls.PictureBoxes.DefaultPictureBox sessionImage;
        private Labels.PrimaryLabel downloadLinkLabel;
        private Labels.PrimaryLabel sessionMaximumParticipants;
        private Labels.PrimaryLabel sessionVersion;
    }
}
