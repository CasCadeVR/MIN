namespace MIN.Desktop.Views.Forms.HelperForms
{
    partial class TestChessForm
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
            heading1Label1 = new MIN.Desktop.Components.Labels.Heading1Label();
            SuspendLayout();
            // 
            // heading1Label1
            // 
            heading1Label1.AutoSize = true;
            heading1Label1.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            heading1Label1.ForeColor = Color.Black;
            heading1Label1.Location = new Point(296, 188);
            heading1Label1.Name = "heading1Label1";
            heading1Label1.Size = new Size(170, 30);
            heading1Label1.TabIndex = 0;
            heading1Label1.Text = "Типо шахматы";
            // 
            // TestChessForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(heading1Label1);
            Name = "TestChessForm";
            Text = "TestChessForm";
            FormClosed += TestChessForm_FormClosed;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Desktop.Components.Labels.Heading1Label heading1Label1;
    }
}