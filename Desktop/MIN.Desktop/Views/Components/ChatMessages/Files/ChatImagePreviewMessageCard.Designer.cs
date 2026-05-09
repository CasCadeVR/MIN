using MIN.Desktop.Components.Labels;

namespace MIN.Desktop.Components
{
    partial class ChatImagePreviewMessageCard
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
            fileNameAndSize = new Heading3Label();
            splitContainerDownload = new SplitContainer();
            downloadProgressBar = new ProgressBar();
            ContentPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainerDownload).BeginInit();
            splitContainerDownload.Panel1.SuspendLayout();
            splitContainerDownload.Panel2.SuspendLayout();
            splitContainerDownload.SuspendLayout();
            SuspendLayout();
            // 
            // ContentPanel
            // 
            ContentPanel.Controls.Add(splitContainerDownload);
            ContentPanel.Size = new Size(361, 423);
            // 
            // fileNameAndSize
            // 
            fileNameAndSize.Dock = DockStyle.Fill;
            fileNameAndSize.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            fileNameAndSize.ForeColor = Color.Black;
            fileNameAndSize.Location = new Point(0, 0);
            fileNameAndSize.Name = "fileNameAndSize";
            fileNameAndSize.Size = new Size(361, 423);
            fileNameAndSize.TabIndex = 0;
            fileNameAndSize.Text = "Имя и размер";
            fileNameAndSize.TextAlign = ContentAlignment.MiddleCenter;
            fileNameAndSize.Click += fileNameAndSize_Click;
            // 
            // splitContainerDownload
            // 
            splitContainerDownload.Dock = DockStyle.Fill;
            splitContainerDownload.FixedPanel = FixedPanel.Panel2;
            splitContainerDownload.IsSplitterFixed = true;
            splitContainerDownload.Location = new Point(0, 0);
            splitContainerDownload.Margin = new Padding(0);
            splitContainerDownload.Name = "splitContainerDownload";
            splitContainerDownload.Orientation = Orientation.Horizontal;
            // 
            // splitContainerDownload.Panel1
            // 
            splitContainerDownload.Panel1.Controls.Add(fileNameAndSize);
            splitContainerDownload.Panel1MinSize = 8;
            // 
            // splitContainerDownload.Panel2
            // 
            splitContainerDownload.Panel2.Controls.Add(downloadProgressBar);
            splitContainerDownload.Panel2Collapsed = true;
            splitContainerDownload.Panel2MinSize = 4;
            splitContainerDownload.Size = new Size(361, 423);
            splitContainerDownload.SplitterDistance = 397;
            splitContainerDownload.SplitterWidth = 1;
            splitContainerDownload.TabIndex = 6;
            // 
            // downloadProgressBar
            // 
            downloadProgressBar.Dock = DockStyle.Bottom;
            downloadProgressBar.Location = new Point(0, 17);
            downloadProgressBar.Margin = new Padding(0);
            downloadProgressBar.Name = "downloadProgressBar";
            downloadProgressBar.Size = new Size(361, 8);
            downloadProgressBar.Step = 1;
            downloadProgressBar.TabIndex = 1;
            // 
            // ChatImagePreviewMessageCard
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            MinimumSize = new Size(179, 22);
            Name = "ChatImagePreviewMessageCard";
            Size = new Size(393, 445);
            ContentPanel.ResumeLayout(false);
            splitContainerDownload.Panel1.ResumeLayout(false);
            splitContainerDownload.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerDownload).EndInit();
            splitContainerDownload.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private SplitContainer splitContainerDownload;
        private ProgressBar downloadProgressBar;
        private Heading3Label fileNameAndSize;
    }
}
