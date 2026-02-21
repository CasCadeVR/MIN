namespace MIN.Desktop
{
    partial class ParticipantCreateForm
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
            connectButton = new MIN.Desktop.Components.CommonButton();
            cancelButton = new MIN.Desktop.Components.InvertedButton();
            tableLayoutPanel1 = new TableLayoutPanel();
            ClassTitleInput = new MIN.Desktop.Components.Labels.Heading3Label();
            participantName = new MIN.Desktop.Components.Controls.TextBoxes.DefaultTextBox();
            ((System.ComponentModel.ISupportInitialize)splitContainer).BeginInit();
            splitContainer.Panel1.SuspendLayout();
            splitContainer.Panel2.SuspendLayout();
            splitContainer.SuspendLayout();
            tableLayoutPanelHeader.SuspendLayout();
            tableLayoutPanelButtons.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
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
            splitContainer.Size = new Size(416, 154);
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
            Title.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            Title.ForeColor = Color.Black;
            Title.Location = new Point(125, 12);
            Title.Name = "Title";
            Title.Size = new Size(165, 30);
            Title.TabIndex = 0;
            Title.Text = "Подключение";
            // 
            // tableLayoutPanelButtons
            // 
            tableLayoutPanelButtons.ColumnCount = 2;
            tableLayoutPanelButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanelButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanelButtons.Controls.Add(connectButton, 0, 0);
            tableLayoutPanelButtons.Controls.Add(cancelButton, 1, 0);
            tableLayoutPanelButtons.Dock = DockStyle.Bottom;
            tableLayoutPanelButtons.Location = new Point(0, 45);
            tableLayoutPanelButtons.Name = "tableLayoutPanelButtons";
            tableLayoutPanelButtons.RowCount = 1;
            tableLayoutPanelButtons.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanelButtons.Size = new Size(416, 50);
            tableLayoutPanelButtons.TabIndex = 2;
            // 
            // connectButton
            // 
            connectButton.BackColor = Color.FromArgb(192, 192, 255);
            connectButton.FlatAppearance.BorderColor = Color.FromArgb(228, 230, 240);
            connectButton.FlatStyle = FlatStyle.Flat;
            connectButton.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 204);
            connectButton.ForeColor = Color.FromArgb(248, 249, 255);
            connectButton.Location = new Point(3, 3);
            connectButton.Name = "connectButton";
            connectButton.Padding = new Padding(8, 4, 8, 4);
            connectButton.Size = new Size(202, 44);
            connectButton.TabIndex = 2;
            connectButton.Text = "Подключиться";
            connectButton.UseVisualStyleBackColor = false;
            connectButton.Click += createButton_Click;
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
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Controls.Add(ClassTitleInput, 0, 0);
            tableLayoutPanel1.Controls.Add(participantName, 1, 0);
            tableLayoutPanel1.Dock = DockStyle.Top;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Margin = new Padding(0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 1;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3333321F));
            tableLayoutPanel1.Size = new Size(416, 46);
            tableLayoutPanel1.TabIndex = 1;
            // 
            // ClassTitleInput
            // 
            ClassTitleInput.Anchor = AnchorStyles.Right;
            ClassTitleInput.AutoSize = true;
            ClassTitleInput.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            ClassTitleInput.ForeColor = Color.FromArgb(0, 0, 0);
            ClassTitleInput.Location = new Point(48, 12);
            ClassTitleInput.Name = "ClassTitleInput";
            ClassTitleInput.Size = new Size(157, 21);
            ClassTitleInput.TabIndex = 0;
            ClassTitleInput.Text = "Введите своё имя: ";
            // 
            // participantName
            // 
            participantName.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            participantName.BackColor = Color.FromArgb(248, 249, 255);
            participantName.BorderStyle = BorderStyle.None;
            participantName.Font = new Font("Segoe UI", 14.25F);
            participantName.ForeColor = Color.Purple;
            participantName.Location = new Point(211, 10);
            participantName.Name = "participantName";
            participantName.PlaceholderText = "Введите имя...";
            participantName.Size = new Size(202, 26);
            participantName.TabIndex = 1;
            // 
            // ParticipantCreateForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(416, 154);
            Controls.Add(splitContainer);
            MinimumSize = new Size(432, 193);
            Name = "ParticipantCreateForm";
            Text = "MIN - Подключение к комнате";
            splitContainer.Panel1.ResumeLayout(false);
            splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer).EndInit();
            splitContainer.ResumeLayout(false);
            tableLayoutPanelHeader.ResumeLayout(false);
            tableLayoutPanelHeader.PerformLayout();
            tableLayoutPanelButtons.ResumeLayout(false);
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private SplitContainer splitContainer;
        private TableLayoutPanel tableLayoutPanelHeader;
        private TableLayoutPanel tableLayoutPanel1;
        private Components.Labels.Heading3Label ClassTitleInput;
        private Components.Labels.Heading1Label Title;
        private Components.Controls.NumericUpDowns.DefaultNumericUpDown classNumber;
        private Components.CommonButton connectButton;
        private Components.Controls.TextBoxes.DefaultTextBox participantName;
        private TableLayoutPanel tableLayoutPanelButtons;
        private Components.InvertedButton cancelButton;
    }
}
