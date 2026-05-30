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
            tableLayoutPanelLabels.Controls.Add(sessionName, 0, 1);
            tableLayoutPanelLabels.Controls.Add(sessionDescription, 0, 2);
            tableLayoutPanelLabels.Controls.Add(sessionImage, 0, 0);
            tableLayoutPanelLabels.Dock = DockStyle.Fill;
            tableLayoutPanelLabels.Location = new Point(5, 5);
            tableLayoutPanelLabels.Margin = new Padding(0);
            tableLayoutPanelLabels.Name = "tableLayoutPanelLabels";
            tableLayoutPanelLabels.RowCount = 3;
            tableLayoutPanelLabels.RowStyles.Add(new RowStyle(SizeType.Absolute, 205F));
            tableLayoutPanelLabels.RowStyles.Add(new RowStyle(SizeType.Absolute, 36F));
            tableLayoutPanelLabels.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanelLabels.Size = new Size(215, 313);
            tableLayoutPanelLabels.TabIndex = 2;
            tableLayoutPanelLabels.Click += card_Click;
            // 
            // sessionName
            // 
            sessionName.Anchor = AnchorStyles.None;
            sessionName.AutoSize = true;
            sessionName.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            sessionName.ForeColor = Color.FromArgb(45, 43, 58);
            sessionName.Location = new Point(64, 212);
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
            sessionDescription.Location = new Point(74, 268);
            sessionDescription.Name = "sessionDescription";
            sessionDescription.Size = new Size(66, 17);
            sessionDescription.TabIndex = 1;
            sessionDescription.Text = "Описание";
            sessionDescription.TextAlign = ContentAlignment.MiddleCenter;
            sessionDescription.Click += card_Click;
            // 
            // sessionImage
            // 
            sessionImage.BackColor = Color.FromArgb(248, 249, 255);
            sessionImage.Dock = DockStyle.Fill;
            sessionImage.Location = new Point(3, 3);
            sessionImage.Name = "sessionImage";
            sessionImage.Size = new Size(209, 199);
            sessionImage.SizeMode = PictureBoxSizeMode.Zoom;
            sessionImage.TabIndex = 2;
            sessionImage.TabStop = false;
            sessionImage.Click += card_Click;
            // 
            // SessionCard
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(tableLayoutPanelLabels);
            Margin = new Padding(0);
            Name = "SessionCard";
            Padding = new Padding(5);
            Size = new Size(225, 323);
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
    }
}
