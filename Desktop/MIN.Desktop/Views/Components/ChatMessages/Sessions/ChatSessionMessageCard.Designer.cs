namespace MIN.Desktop.Components
{
    partial class ChatSessionMessageCard
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
            sessionName = new MIN.Desktop.Components.Labels.Heading3Label();
            sessionImage = new MIN.Desktop.Components.Controls.PictureBoxes.DefaultPictureBox();
            joinButton = new MIN.Desktop.Components.Controls.Buttons.CommonButton();
            ContentPanel.SuspendLayout();
            tableLayoutPanelLabels.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)sessionImage).BeginInit();
            SuspendLayout();
            // 
            // ContentPanel
            // 
            ContentPanel.Controls.Add(tableLayoutPanelLabels);
            ContentPanel.Size = new Size(318, 128);
            // 
            // tableLayoutPanelLabels
            // 
            tableLayoutPanelLabels.ColumnCount = 2;
            tableLayoutPanelLabels.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 128F));
            tableLayoutPanelLabels.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanelLabels.Controls.Add(sessionName, 1, 0);
            tableLayoutPanelLabels.Controls.Add(sessionImage, 0, 0);
            tableLayoutPanelLabels.Controls.Add(joinButton, 1, 1);
            tableLayoutPanelLabels.Dock = DockStyle.Fill;
            tableLayoutPanelLabels.Location = new Point(0, 0);
            tableLayoutPanelLabels.Margin = new Padding(0);
            tableLayoutPanelLabels.Name = "tableLayoutPanelLabels";
            tableLayoutPanelLabels.RowCount = 2;
            tableLayoutPanelLabels.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
            tableLayoutPanelLabels.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanelLabels.Size = new Size(318, 128);
            tableLayoutPanelLabels.TabIndex = 3;
            // 
            // sessionName
            // 
            sessionName.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            sessionName.AutoEllipsis = true;
            sessionName.AutoSize = true;
            sessionName.BackColor = Color.Transparent;
            sessionName.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 204);
            sessionName.ForeColor = Color.FromArgb(0, 0, 0);
            sessionName.Location = new Point(131, 28);
            sessionName.MaximumSize = new Size(0, 20);
            sessionName.Name = "sessionName";
            sessionName.Size = new Size(184, 20);
            sessionName.TabIndex = 3;
            sessionName.Text = "Имя сессии";
            sessionName.TextAlign = ContentAlignment.BottomLeft;
            // 
            // sessionImage
            // 
            sessionImage.BackColor = Color.Gray;
            sessionImage.Dock = DockStyle.Fill;
            sessionImage.Location = new Point(3, 3);
            sessionImage.Name = "sessionImage";
            tableLayoutPanelLabels.SetRowSpan(sessionImage, 2);
            sessionImage.Size = new Size(122, 122);
            sessionImage.SizeMode = PictureBoxSizeMode.StretchImage;
            sessionImage.TabIndex = 6;
            sessionImage.TabStop = false;
            // 
            // joinButton
            // 
            joinButton.BackColor = Color.FromArgb(167, 157, 255);
            joinButton.FlatAppearance.BorderColor = Color.FromArgb(228, 230, 240);
            joinButton.FlatStyle = FlatStyle.Flat;
            joinButton.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 204);
            joinButton.ForeColor = Color.FromArgb(248, 249, 255);
            joinButton.Location = new Point(131, 51);
            joinButton.Name = "joinButton";
            joinButton.Size = new Size(184, 58);
            joinButton.TabIndex = 7;
            joinButton.Text = "Присоединиться";
            joinButton.UseVisualStyleBackColor = false;
            joinButton.Click += joinButton_Click;
            // 
            // ChatSessionMessageCard
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            MaximumSize = new Size(350, 150);
            MinimumSize = new Size(179, 22);
            Name = "ChatSessionMessageCard";
            Size = new Size(350, 150);
            ContentPanel.ResumeLayout(false);
            tableLayoutPanelLabels.ResumeLayout(false);
            tableLayoutPanelLabels.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)sessionImage).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanelLabels;
        private Labels.Heading3Label sessionName;
        private Controls.PictureBoxes.DefaultPictureBox sessionImage;
        private Controls.Buttons.CommonButton joinButton;
    }
}
