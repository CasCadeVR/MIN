using MIN.Desktop.Components.Controls.Buttons;
using MIN.Desktop.Components.Controls.NumericUpDowns;
using MIN.Desktop.Components.Controls.TextBoxes;
using MIN.Desktop.Components.Labels;

namespace MIN.Desktop.Views.Forms.HelperForms
{
    partial class SessionChoosingForm
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
            flowPanel = new MIN.Desktop.Components.Controls.FlowLayoutPanels.NoHorizontalScrollListView();
            tableLayoutPanelButtons = new TableLayoutPanel();
            selectButton = new CommonButton();
            cancelButton = new InvertedButton();
            ((System.ComponentModel.ISupportInitialize)splitContainer).BeginInit();
            splitContainer.Panel1.SuspendLayout();
            splitContainer.Panel2.SuspendLayout();
            splitContainer.SuspendLayout();
            tableLayoutPanelHeader.SuspendLayout();
            tableLayoutPanelButtons.SuspendLayout();
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
            splitContainer.Panel2.Controls.Add(flowPanel);
            splitContainer.Panel2.Controls.Add(tableLayoutPanelButtons);
            splitContainer.Size = new Size(552, 440);
            splitContainer.SplitterDistance = 55;
            splitContainer.TabIndex = 0;
            // 
            // tableLayoutPanelHeader
            // 
            tableLayoutPanelHeader.ColumnCount = 1;
            tableLayoutPanelHeader.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanelHeader.Controls.Add(Title, 0, 0);
            tableLayoutPanelHeader.Dock = DockStyle.Fill;
            tableLayoutPanelHeader.Location = new Point(0, 0);
            tableLayoutPanelHeader.Name = "tableLayoutPanelHeader";
            tableLayoutPanelHeader.RowCount = 1;
            tableLayoutPanelHeader.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanelHeader.Size = new Size(552, 55);
            tableLayoutPanelHeader.TabIndex = 0;
            // 
            // Title
            // 
            Title.Anchor = AnchorStyles.None;
            Title.AutoSize = true;
            Title.Font = new Font("Segoe UI", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 204);
            Title.ForeColor = Color.Black;
            Title.Location = new Point(168, 12);
            Title.Name = "Title";
            Title.Size = new Size(216, 30);
            Title.TabIndex = 0;
            Title.Text = "Выбор активностей";
            // 
            // flowPanel
            // 
            flowPanel.Dock = DockStyle.Fill;
            flowPanel.Location = new Point(0, 0);
            flowPanel.Margin = new Padding(0);
            flowPanel.Name = "flowPanel";
            flowPanel.Size = new Size(552, 331);
            flowPanel.TabIndex = 3;
            // 
            // tableLayoutPanelButtons
            // 
            tableLayoutPanelButtons.ColumnCount = 2;
            tableLayoutPanelButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanelButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanelButtons.Controls.Add(selectButton, 0, 0);
            tableLayoutPanelButtons.Controls.Add(cancelButton, 1, 0);
            tableLayoutPanelButtons.Dock = DockStyle.Bottom;
            tableLayoutPanelButtons.Location = new Point(0, 331);
            tableLayoutPanelButtons.Name = "tableLayoutPanelButtons";
            tableLayoutPanelButtons.RowCount = 1;
            tableLayoutPanelButtons.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanelButtons.Size = new Size(552, 50);
            tableLayoutPanelButtons.TabIndex = 2;
            // 
            // selectButton
            // 
            selectButton.BackColor = Color.FromArgb(192, 192, 255);
            selectButton.Dock = DockStyle.Fill;
            selectButton.Enabled = false;
            selectButton.FlatAppearance.BorderColor = Color.FromArgb(228, 230, 240);
            selectButton.FlatStyle = FlatStyle.Flat;
            selectButton.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 204);
            selectButton.ForeColor = Color.FromArgb(248, 249, 255);
            selectButton.Location = new Point(3, 3);
            selectButton.Name = "selectButton";
            selectButton.Padding = new Padding(8, 4, 8, 4);
            selectButton.Size = new Size(270, 44);
            selectButton.TabIndex = 2;
            selectButton.Text = "Начать";
            selectButton.UseVisualStyleBackColor = false;
            selectButton.Click += selectButton_Click;
            // 
            // cancelButton
            // 
            cancelButton.BackColor = Color.FromArgb(248, 249, 255);
            cancelButton.DialogResult = DialogResult.Cancel;
            cancelButton.Dock = DockStyle.Fill;
            cancelButton.FlatAppearance.BorderColor = Color.FromArgb(167, 157, 255);
            cancelButton.FlatAppearance.BorderSize = 2;
            cancelButton.FlatStyle = FlatStyle.Flat;
            cancelButton.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            cancelButton.ForeColor = Color.FromArgb(167, 157, 255);
            cancelButton.Location = new Point(279, 3);
            cancelButton.Name = "cancelButton";
            cancelButton.Padding = new Padding(8, 4, 8, 4);
            cancelButton.Size = new Size(270, 44);
            cancelButton.TabIndex = 3;
            cancelButton.Text = "Отмена";
            cancelButton.UseVisualStyleBackColor = false;
            cancelButton.Click += cancelButton_Click;
            // 
            // SessionChoosingForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = cancelButton;
            ClientSize = new Size(552, 440);
            Controls.Add(splitContainer);
            MinimumSize = new Size(0, 479);
            Name = "SessionChoosingForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "MIN - Выбор активностей";
            splitContainer.Panel1.ResumeLayout(false);
            splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer).EndInit();
            splitContainer.ResumeLayout(false);
            tableLayoutPanelHeader.ResumeLayout(false);
            tableLayoutPanelHeader.PerformLayout();
            tableLayoutPanelButtons.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private SplitContainer splitContainer;
        private TableLayoutPanel tableLayoutPanelHeader;
        private Heading1Label Title;
        private CommonButton selectButton;
        private TableLayoutPanel tableLayoutPanelButtons;
        private InvertedButton cancelButton;
        private Desktop.Components.Controls.FlowLayoutPanels.NoHorizontalScrollListView flowPanel;
    }
}
