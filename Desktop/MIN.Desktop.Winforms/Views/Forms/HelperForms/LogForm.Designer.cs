using MIN.Desktop.Components.Controls.ListBoxes;
using MIN.Desktop.Components.Labels;
using MIN.Desktop.Winforms.Properties;

namespace MIN.Desktop.Views.Forms.HelperForms
{
    partial class LogForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            splitContainer = new SplitContainer();
            tableLayoutPanelHeader = new TableLayoutPanel();
            Title = new Heading1Label();
            loadMoreButton = new MIN.Desktop.Components.Controls.Buttons.CommonButton();
            scrollUpButton = new MIN.Desktop.Components.Controls.Buttons.CommonButton();
            logListBox = new DefaultListBox();
            ((System.ComponentModel.ISupportInitialize)splitContainer).BeginInit();
            splitContainer.Panel1.SuspendLayout();
            splitContainer.Panel2.SuspendLayout();
            splitContainer.SuspendLayout();
            tableLayoutPanelHeader.SuspendLayout();
            SuspendLayout();
            // 
            // splitContainer
            // 
            splitContainer.Dock = DockStyle.Fill;
            splitContainer.FixedPanel = FixedPanel.Panel1;
            splitContainer.IsSplitterFixed = true;
            splitContainer.Location = new Point(0, 0);
            splitContainer.Name = "splitContainer";
            splitContainer.Orientation = Orientation.Horizontal;
            // 
            // splitContainer.Panel1
            // 
            splitContainer.Panel1.Controls.Add(tableLayoutPanelHeader);
            // 
            // splitContainer.Panel2
            // 
            splitContainer.Panel2.Controls.Add(logListBox);
            splitContainer.Size = new Size(776, 186);
            splitContainer.SplitterDistance = 55;
            splitContainer.TabIndex = 1;
            // 
            // tableLayoutPanelHeader
            // 
            tableLayoutPanelHeader.ColumnCount = 3;
            tableLayoutPanelHeader.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanelHeader.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 55F));
            tableLayoutPanelHeader.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 55F));
            tableLayoutPanelHeader.Controls.Add(Title, 0, 0);
            tableLayoutPanelHeader.Controls.Add(loadMoreButton, 2, 0);
            tableLayoutPanelHeader.Controls.Add(scrollUpButton, 1, 0);
            tableLayoutPanelHeader.Dock = DockStyle.Fill;
            tableLayoutPanelHeader.Location = new Point(0, 0);
            tableLayoutPanelHeader.Name = "tableLayoutPanelHeader";
            tableLayoutPanelHeader.RowCount = 1;
            tableLayoutPanelHeader.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanelHeader.Size = new Size(776, 55);
            tableLayoutPanelHeader.TabIndex = 0;
            // 
            // Title
            // 
            Title.Anchor = AnchorStyles.None;
            Title.AutoSize = true;
            Title.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            Title.ForeColor = Color.Black;
            Title.Location = new Point(300, 12);
            Title.Name = "Title";
            Title.Size = new Size(65, 30);
            Title.TabIndex = 0;
            Title.Text = "Логи";
            // 
            // loadMoreButton
            // 
            loadMoreButton.BackColor = Color.FromArgb(167, 157, 255);
            loadMoreButton.BackgroundImage = Resources.download;
            loadMoreButton.BackgroundImageLayout = ImageLayout.Zoom;
            loadMoreButton.Dock = DockStyle.Fill;
            loadMoreButton.FlatAppearance.BorderColor = Color.FromArgb(228, 230, 240);
            loadMoreButton.FlatStyle = FlatStyle.Flat;
            loadMoreButton.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            loadMoreButton.ForeColor = Color.FromArgb(248, 249, 255);
            loadMoreButton.Location = new Point(724, 3);
            loadMoreButton.Name = "loadMoreButton";
            loadMoreButton.Size = new Size(49, 49);
            loadMoreButton.TabIndex = 1;
            loadMoreButton.UseVisualStyleBackColor = false;
            loadMoreButton.Click += loadMoreButton_Click;
            // 
            // scrollUpButton
            // 
            scrollUpButton.BackColor = Color.FromArgb(167, 157, 255);
            scrollUpButton.BackgroundImage = Resources.up;
            scrollUpButton.BackgroundImageLayout = ImageLayout.Zoom;
            scrollUpButton.Dock = DockStyle.Fill;
            scrollUpButton.FlatAppearance.BorderColor = Color.FromArgb(228, 230, 240);
            scrollUpButton.FlatStyle = FlatStyle.Flat;
            scrollUpButton.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            scrollUpButton.ForeColor = Color.FromArgb(248, 249, 255);
            scrollUpButton.Location = new Point(669, 3);
            scrollUpButton.Name = "scrollUpButton";
            scrollUpButton.Size = new Size(49, 49);
            scrollUpButton.TabIndex = 2;
            scrollUpButton.UseVisualStyleBackColor = false;
            scrollUpButton.Click += scrollUpButton_Click;
            // 
            // logListBox
            // 
            logListBox.BackColor = Color.FromArgb(248, 249, 255);
            logListBox.Dock = DockStyle.Fill;
            logListBox.Font = new Font("Segoe UI", 9.75F);
            logListBox.ForeColor = Color.FromArgb(45, 43, 58);
            logListBox.FormattingEnabled = true;
            logListBox.HorizontalScrollbar = true;
            logListBox.IntegralHeight = false;
            logListBox.ItemHeight = 17;
            logListBox.Location = new Point(0, 0);
            logListBox.Name = "logListBox";
            logListBox.ScrollAlwaysVisible = true;
            logListBox.Size = new Size(776, 127);
            logListBox.TabIndex = 0;
            // 
            // LogForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(776, 186);
            Controls.Add(splitContainer);
            Margin = new Padding(3, 4, 3, 4);
            MinimumSize = new Size(432, 191);
            Name = "LogForm";
            StartPosition = FormStartPosition.Manual;
            Text = "MIN - Логи";
            FormClosing += LogForm_FormClosing;
            Load += LogForm_Load;
            splitContainer.Panel1.ResumeLayout(false);
            splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer).EndInit();
            splitContainer.ResumeLayout(false);
            tableLayoutPanelHeader.ResumeLayout(false);
            tableLayoutPanelHeader.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        private SplitContainer splitContainer;
        private TableLayoutPanel tableLayoutPanelHeader;
        private Heading1Label Title;
        private DefaultListBox logListBox;
        private Desktop.Components.Controls.Buttons.CommonButton loadMoreButton;
        private Desktop.Components.Controls.Buttons.CommonButton scrollUpButton;
    }
}
