namespace MIN.Desktop.Components
{
    partial class RoomCard
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
            splitContainer = new SplitContainer();
            tableLayoutPanelHeader = new TableLayoutPanel();
            Title = new MIN.Desktop.Components.Labels.Heading1Label();
            connectButton = new CommonButton();
            tableLayoutPanelLabels = new TableLayoutPanel();
            participantsInfo = new MIN.Desktop.Components.Labels.Heading3Label();
            heading3Label3 = new MIN.Desktop.Components.Labels.Heading3Label();
            computer = new MIN.Desktop.Components.Labels.Heading3Label();
            classroom = new MIN.Desktop.Components.Labels.Heading3Label();
            heading3Label1 = new MIN.Desktop.Components.Labels.Heading3Label();
            hostName = new MIN.Desktop.Components.Labels.Heading3Label();
            ClassTitleInput = new MIN.Desktop.Components.Labels.Heading3Label();
            heading3Label2 = new MIN.Desktop.Components.Labels.Heading3Label();
            ((System.ComponentModel.ISupportInitialize)splitContainer).BeginInit();
            splitContainer.Panel1.SuspendLayout();
            splitContainer.Panel2.SuspendLayout();
            splitContainer.SuspendLayout();
            tableLayoutPanelHeader.SuspendLayout();
            tableLayoutPanelLabels.SuspendLayout();
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
            splitContainer.Panel2.Controls.Add(connectButton);
            splitContainer.Panel2.Controls.Add(tableLayoutPanelLabels);
            splitContainer.Size = new Size(311, 227);
            splitContainer.SplitterDistance = 55;
            splitContainer.TabIndex = 1;
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
            tableLayoutPanelHeader.Size = new Size(311, 55);
            tableLayoutPanelHeader.TabIndex = 0;
            // 
            // Title
            // 
            Title.Anchor = AnchorStyles.None;
            Title.AutoSize = true;
            Title.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            Title.ForeColor = Color.Black;
            Title.Location = new Point(89, 12);
            Title.Name = "Title";
            Title.Size = new Size(132, 30);
            Title.TabIndex = 0;
            Title.Text = "Комната \"\"";
            // 
            // connectButton
            // 
            connectButton.BackColor = Color.FromArgb(192, 192, 255);
            connectButton.Cursor = Cursors.Hand;
            connectButton.Dock = DockStyle.Bottom;
            connectButton.FlatAppearance.BorderColor = Color.FromArgb(228, 230, 240);
            connectButton.FlatStyle = FlatStyle.Flat;
            connectButton.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 204);
            connectButton.ForeColor = Color.FromArgb(248, 249, 255);
            connectButton.Location = new Point(0, 124);
            connectButton.Name = "connectButton";
            connectButton.Padding = new Padding(8, 4, 8, 4);
            connectButton.Size = new Size(311, 44);
            connectButton.TabIndex = 2;
            connectButton.Text = "Присоединиться";
            connectButton.UseVisualStyleBackColor = false;
            connectButton.Click += connectButton_Click;
            // 
            // tableLayoutPanelLabels
            // 
            tableLayoutPanelLabels.ColumnCount = 2;
            tableLayoutPanelLabels.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanelLabels.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanelLabels.Controls.Add(participantsInfo, 1, 3);
            tableLayoutPanelLabels.Controls.Add(heading3Label3, 0, 3);
            tableLayoutPanelLabels.Controls.Add(computer, 1, 2);
            tableLayoutPanelLabels.Controls.Add(classroom, 1, 1);
            tableLayoutPanelLabels.Controls.Add(heading3Label1, 0, 1);
            tableLayoutPanelLabels.Controls.Add(hostName, 1, 0);
            tableLayoutPanelLabels.Controls.Add(ClassTitleInput, 0, 0);
            tableLayoutPanelLabels.Controls.Add(heading3Label2, 0, 2);
            tableLayoutPanelLabels.Dock = DockStyle.Top;
            tableLayoutPanelLabels.Location = new Point(0, 0);
            tableLayoutPanelLabels.Margin = new Padding(0);
            tableLayoutPanelLabels.Name = "tableLayoutPanelLabels";
            tableLayoutPanelLabels.RowCount = 4;
            tableLayoutPanelLabels.RowStyles.Add(new RowStyle(SizeType.Percent, 25.0006237F));
            tableLayoutPanelLabels.RowStyles.Add(new RowStyle(SizeType.Percent, 25.0006275F));
            tableLayoutPanelLabels.RowStyles.Add(new RowStyle(SizeType.Percent, 25.0006237F));
            tableLayoutPanelLabels.RowStyles.Add(new RowStyle(SizeType.Percent, 24.9981289F));
            tableLayoutPanelLabels.Size = new Size(311, 121);
            tableLayoutPanelLabels.TabIndex = 1;
            // 
            // participantsInfo
            // 
            participantsInfo.Anchor = AnchorStyles.Left;
            participantsInfo.AutoSize = true;
            participantsInfo.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            participantsInfo.ForeColor = Color.FromArgb(0, 0, 0);
            participantsInfo.Location = new Point(158, 97);
            participantsInfo.Name = "participantsInfo";
            participantsInfo.Size = new Size(74, 17);
            participantsInfo.TabIndex = 7;
            participantsInfo.Text = "Загрузка...";
            // 
            // heading3Label3
            // 
            heading3Label3.Anchor = AnchorStyles.Right;
            heading3Label3.AutoSize = true;
            heading3Label3.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            heading3Label3.ForeColor = Color.FromArgb(0, 0, 0);
            heading3Label3.Location = new Point(53, 97);
            heading3Label3.Name = "heading3Label3";
            heading3Label3.Size = new Size(99, 17);
            heading3Label3.TabIndex = 6;
            heading3Label3.Text = "Подключено: ";
            // 
            // computer
            // 
            computer.Anchor = AnchorStyles.Left;
            computer.AutoSize = true;
            computer.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            computer.ForeColor = Color.FromArgb(0, 0, 0);
            computer.Location = new Point(158, 66);
            computer.Name = "computer";
            computer.Size = new Size(74, 17);
            computer.TabIndex = 5;
            computer.Text = "Загрузка...";
            // 
            // classroom
            // 
            classroom.Anchor = AnchorStyles.Left;
            classroom.AutoSize = true;
            classroom.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            classroom.ForeColor = Color.FromArgb(0, 0, 0);
            classroom.Location = new Point(158, 36);
            classroom.Name = "classroom";
            classroom.Size = new Size(74, 17);
            classroom.TabIndex = 4;
            classroom.Text = "Загрузка...";
            // 
            // heading3Label1
            // 
            heading3Label1.Anchor = AnchorStyles.Right;
            heading3Label1.AutoSize = true;
            heading3Label1.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            heading3Label1.ForeColor = Color.FromArgb(0, 0, 0);
            heading3Label1.Location = new Point(84, 36);
            heading3Label1.Name = "heading3Label1";
            heading3Label1.Size = new Size(68, 17);
            heading3Label1.TabIndex = 2;
            heading3Label1.Text = "Кабинет: ";
            // 
            // hostName
            // 
            hostName.Anchor = AnchorStyles.Left;
            hostName.AutoSize = true;
            hostName.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            hostName.ForeColor = Color.FromArgb(0, 0, 0);
            hostName.Location = new Point(158, 6);
            hostName.Name = "hostName";
            hostName.Size = new Size(74, 17);
            hostName.TabIndex = 1;
            hostName.Text = "Загрузка...";
            // 
            // ClassTitleInput
            // 
            ClassTitleInput.Anchor = AnchorStyles.Right;
            ClassTitleInput.AutoSize = true;
            ClassTitleInput.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            ClassTitleInput.ForeColor = Color.FromArgb(0, 0, 0);
            ClassTitleInput.Location = new Point(10, 6);
            ClassTitleInput.Name = "ClassTitleInput";
            ClassTitleInput.Size = new Size(142, 17);
            ClassTitleInput.TabIndex = 0;
            ClassTitleInput.Text = "Создатель комнаты: ";
            // 
            // heading3Label2
            // 
            heading3Label2.Anchor = AnchorStyles.Right;
            heading3Label2.AutoSize = true;
            heading3Label2.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            heading3Label2.ForeColor = Color.FromArgb(0, 0, 0);
            heading3Label2.Location = new Point(36, 66);
            heading3Label2.Name = "heading3Label2";
            heading3Label2.Size = new Size(116, 17);
            heading3Label2.TabIndex = 3;
            heading3Label2.Text = "№ Компьютера: ";
            // 
            // RoomCard
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(splitContainer);
            Margin = new Padding(0, 0, 0, 5);
            Name = "RoomCard";
            Size = new Size(311, 227);
            splitContainer.Panel1.ResumeLayout(false);
            splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer).EndInit();
            splitContainer.ResumeLayout(false);
            tableLayoutPanelHeader.ResumeLayout(false);
            tableLayoutPanelHeader.PerformLayout();
            tableLayoutPanelLabels.ResumeLayout(false);
            tableLayoutPanelLabels.PerformLayout();
            ResumeLayout(false);

        }

        #endregion

        private SplitContainer splitContainer;
        private TableLayoutPanel tableLayoutPanelHeader;
        private Labels.Heading1Label Title;
        private CommonButton connectButton;
        private TableLayoutPanel tableLayoutPanelLabels;
        private Labels.Heading3Label ClassTitleInput;
        private Labels.Heading3Label computer;
        private Labels.Heading3Label classroom;
        private Labels.Heading3Label heading3Label1;
        private Labels.Heading3Label hostName;
        private Labels.Heading3Label heading3Label2;
        private Labels.Heading3Label participantsInfo;
        private Labels.Heading3Label heading3Label3;
    }
}
