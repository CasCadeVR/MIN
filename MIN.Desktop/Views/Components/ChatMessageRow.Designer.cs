namespace MIN.Desktop.Views.Components
{
    partial class ChatMessageRow
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
            container = new TableLayoutPanel();
            SuspendLayout();
            // 
            // container
            // 
            container.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            container.ColumnCount = 1;
            container.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            container.Dock = DockStyle.Fill;
            container.Location = new Point(0, 0);
            container.Margin = new Padding(0);
            container.Name = "container";
            container.RowCount = 1;
            container.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            container.Size = new Size(622, 70);
            container.TabIndex = 0;
            // 
            // ChatMessageRow
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            Controls.Add(container);
            Margin = new Padding(0);
            Name = "ChatMessageRow";
            Size = new Size(622, 70);
            ResumeLayout(false);
        }

        #endregion

        public TableLayoutPanel container;
    }
}
