using MIN.Desktop.Components.Controls.Buttons;
using MIN.Desktop.Components.Controls.NumericUpDowns;
using MIN.Desktop.Components.Controls.TextBoxes;
using MIN.Desktop.Components.Labels;

namespace MIN.Desktop.Views.Forms.HelperForms
{
    partial class RoomCreateForm
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
            Title = new MIN.Desktop.Components.Labels.Heading1Label();
            tableLayoutPanelButtons = new TableLayoutPanel();
            createButton = new MIN.Desktop.Components.Controls.Buttons.CommonButton();
            cancelButton = new MIN.Desktop.Components.Controls.Buttons.InvertedButton();
            tableLayoutPanel1 = new TableLayoutPanel();
            roomMaximumCount = new MIN.Desktop.Components.Controls.NumericUpDowns.DefaultNumericUpDown();
            heading3Label1 = new MIN.Desktop.Components.Labels.Heading3Label();
            ClassTitleInput = new MIN.Desktop.Components.Labels.Heading3Label();
            roomName = new MIN.Desktop.Components.Controls.TextBoxes.DefaultTextBox();
            ((System.ComponentModel.ISupportInitialize)splitContainer).BeginInit();
            splitContainer.Panel1.SuspendLayout();
            splitContainer.Panel2.SuspendLayout();
            splitContainer.SuspendLayout();
            tableLayoutPanelHeader.SuspendLayout();
            tableLayoutPanelButtons.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)roomMaximumCount).BeginInit();
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
            splitContainer.Panel2.Controls.Add(tableLayoutPanelButtons);
            splitContainer.Panel2.Controls.Add(tableLayoutPanel1);
            splitContainer.Size = new Size(416, 202);
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
            tableLayoutPanelHeader.Size = new Size(416, 55);
            tableLayoutPanelHeader.TabIndex = 0;
            // 
            // Title
            // 
            Title.Anchor = AnchorStyles.None;
            Title.AutoSize = true;
            Title.Font = new Font("Segoe UI", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 204);
            Title.ForeColor = Color.Black;
            Title.Location = new Point(104, 12);
            Title.Name = "Title";
            Title.Size = new Size(208, 30);
            Title.TabIndex = 0;
            Title.Text = "Создание комнаты";
            // 
            // tableLayoutPanelButtons
            // 
            tableLayoutPanelButtons.ColumnCount = 2;
            tableLayoutPanelButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanelButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanelButtons.Controls.Add(createButton, 0, 0);
            tableLayoutPanelButtons.Controls.Add(cancelButton, 1, 0);
            tableLayoutPanelButtons.Dock = DockStyle.Bottom;
            tableLayoutPanelButtons.Location = new Point(0, 93);
            tableLayoutPanelButtons.Name = "tableLayoutPanelButtons";
            tableLayoutPanelButtons.RowCount = 1;
            tableLayoutPanelButtons.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanelButtons.Size = new Size(416, 50);
            tableLayoutPanelButtons.TabIndex = 2;
            // 
            // createButton
            // 
            createButton.Anchor = AnchorStyles.Right;
            createButton.BackColor = Color.FromArgb(192, 192, 255);
            createButton.FlatAppearance.BorderColor = Color.FromArgb(228, 230, 240);
            createButton.FlatStyle = FlatStyle.Flat;
            createButton.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 204);
            createButton.ForeColor = Color.FromArgb(248, 249, 255);
            createButton.Location = new Point(3, 3);
            createButton.Name = "createButton";
            createButton.Padding = new Padding(8, 4, 8, 4);
            createButton.Size = new Size(202, 44);
            createButton.TabIndex = 2;
            createButton.Text = "Создать";
            createButton.UseVisualStyleBackColor = false;
            createButton.Click += createButton_Click;
            // 
            // cancelButton
            // 
            cancelButton.Anchor = AnchorStyles.Left;
            cancelButton.BackColor = Color.FromArgb(248, 249, 255);
            cancelButton.DialogResult = DialogResult.Cancel;
            cancelButton.FlatAppearance.BorderColor = Color.FromArgb(167, 157, 255);
            cancelButton.FlatAppearance.BorderSize = 2;
            cancelButton.FlatStyle = FlatStyle.Flat;
            cancelButton.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            cancelButton.ForeColor = Color.FromArgb(167, 157, 255);
            cancelButton.Location = new Point(211, 3);
            cancelButton.Name = "cancelButton";
            cancelButton.Padding = new Padding(8, 4, 8, 4);
            cancelButton.Size = new Size(202, 44);
            cancelButton.TabIndex = 3;
            cancelButton.Text = "Отмена";
            cancelButton.UseVisualStyleBackColor = false;
            cancelButton.Click += cancelButton_Click;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Controls.Add(roomMaximumCount, 1, 1);
            tableLayoutPanel1.Controls.Add(heading3Label1, 0, 1);
            tableLayoutPanel1.Controls.Add(ClassTitleInput, 0, 0);
            tableLayoutPanel1.Controls.Add(roomName, 1, 0);
            tableLayoutPanel1.Dock = DockStyle.Top;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Margin = new Padding(0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3333321F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3333321F));
            tableLayoutPanel1.Size = new Size(416, 94);
            tableLayoutPanel1.TabIndex = 1;
            // 
            // roomMaximumCount
            // 
            roomMaximumCount.Anchor = AnchorStyles.Left;
            roomMaximumCount.BackColor = Color.White;
            roomMaximumCount.BorderStyle = BorderStyle.None;
            roomMaximumCount.Font = new Font("Segoe UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            roomMaximumCount.ForeColor = Color.Purple;
            roomMaximumCount.Location = new Point(211, 56);
            roomMaximumCount.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
            roomMaximumCount.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            roomMaximumCount.Name = "roomMaximumCount";
            roomMaximumCount.Size = new Size(46, 29);
            roomMaximumCount.TabIndex = 1;
            roomMaximumCount.Value = new decimal(new int[] { 2, 0, 0, 0 });
            // 
            // heading3Label1
            // 
            heading3Label1.Anchor = AnchorStyles.Right;
            heading3Label1.AutoSize = true;
            heading3Label1.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            heading3Label1.ForeColor = Color.FromArgb(0, 0, 0);
            heading3Label1.Location = new Point(3, 60);
            heading3Label1.Name = "heading3Label1";
            heading3Label1.Size = new Size(202, 21);
            heading3Label1.TabIndex = 0;
            heading3Label1.Text = "Количество участников: ";
            // 
            // ClassTitleInput
            // 
            ClassTitleInput.Anchor = AnchorStyles.Right;
            ClassTitleInput.AutoSize = true;
            ClassTitleInput.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            ClassTitleInput.ForeColor = Color.FromArgb(0, 0, 0);
            ClassTitleInput.Location = new Point(79, 13);
            ClassTitleInput.Name = "ClassTitleInput";
            ClassTitleInput.Size = new Size(126, 21);
            ClassTitleInput.TabIndex = 0;
            ClassTitleInput.Text = "Имя комнаты: ";
            // 
            // roomName
            // 
            roomName.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            roomName.BackColor = Color.FromArgb(248, 249, 255);
            roomName.BorderStyle = BorderStyle.None;
            roomName.Font = new Font("Segoe UI", 14.25F);
            roomName.ForeColor = Color.Purple;
            roomName.Location = new Point(211, 10);
            roomName.Name = "roomName";
            roomName.PlaceholderText = "Введите имя...";
            roomName.Size = new Size(202, 26);
            roomName.TabIndex = 1;
            // 
            // RoomCreateForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(416, 202);
            Controls.Add(splitContainer);
            MinimumSize = new Size(432, 241);
            Name = "RoomCreateForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "MIN - Создание комнаты";
            splitContainer.Panel1.ResumeLayout(false);
            splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer).EndInit();
            splitContainer.ResumeLayout(false);
            tableLayoutPanelHeader.ResumeLayout(false);
            tableLayoutPanelHeader.PerformLayout();
            tableLayoutPanelButtons.ResumeLayout(false);
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)roomMaximumCount).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private SplitContainer splitContainer;
        private TableLayoutPanel tableLayoutPanelHeader;
        private TableLayoutPanel tableLayoutPanel1;
        private Heading3Label ClassTitleInput;
        private Heading1Label Title;
        private CommonButton createButton;
        private Heading3Label heading3Label1;
        private DefaultNumericUpDown roomMaximumCount;
        private DefaultTextBox roomName;
        private TableLayoutPanel tableLayoutPanelButtons;
        private InvertedButton cancelButton;
    }
}
