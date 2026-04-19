namespace MIN.Desktop.Views.Forms.HelperForms
{
    partial class LoadingForm
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
            loadingTitle = new MIN.Desktop.Components.Labels.Heading3Label();
            tableLayoutPanel = new TableLayoutPanel();
            tableLayoutPanel.SuspendLayout();
            SuspendLayout();
            // 
            // loadingTitle
            // 
            loadingTitle.Anchor = AnchorStyles.None;
            loadingTitle.AutoSize = true;
            loadingTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            loadingTitle.ForeColor = Color.FromArgb(45, 43, 58);
            loadingTitle.Location = new Point(28, 22);
            loadingTitle.Name = "loadingTitle";
            loadingTitle.Size = new Size(147, 21);
            loadingTitle.TabIndex = 0;
            loadingTitle.Text = "Подключение . . .";
            loadingTitle.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel
            // 
            tableLayoutPanel.ColumnCount = 1;
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel.Controls.Add(loadingTitle, 0, 0);
            tableLayoutPanel.Dock = DockStyle.Fill;
            tableLayoutPanel.Location = new Point(0, 0);
            tableLayoutPanel.Name = "tableLayoutPanel";
            tableLayoutPanel.RowCount = 1;
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel.Size = new Size(204, 66);
            tableLayoutPanel.TabIndex = 1;
            // 
            // LoadingForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(204, 66);
            Controls.Add(tableLayoutPanel);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Name = "LoadingForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Загрузка";
            FormClosing += LoadingForm_FormClosing;
            tableLayoutPanel.ResumeLayout(false);
            tableLayoutPanel.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Desktop.Components.Labels.Heading3Label loadingTitle;
        private TableLayoutPanel tableLayoutPanel;
    }
}