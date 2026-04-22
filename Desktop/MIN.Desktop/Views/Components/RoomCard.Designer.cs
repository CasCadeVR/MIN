using MIN.Desktop.Components.Controls.Buttons;

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
            createdAt = new MIN.Desktop.Components.Labels.Heading3Label();
            labelCreatedAt = new MIN.Desktop.Components.Labels.CaptionLabel();
            creatorLabel = new MIN.Desktop.Components.Labels.CaptionLabel();
            hostName = new MIN.Desktop.Components.Labels.Heading3Label();
            currentlyConnectedLabel = new MIN.Desktop.Components.Labels.CaptionLabel();
            participantsInfo = new MIN.Desktop.Components.Labels.Heading3Label();
            computerNumberLabel = new MIN.Desktop.Components.Labels.CaptionLabel();
            computer = new MIN.Desktop.Components.Labels.Heading3Label();
            roomNumberLabel = new MIN.Desktop.Components.Labels.CaptionLabel();
            classroom = new MIN.Desktop.Components.Labels.Heading3Label();
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
            splitContainer.Size = new Size(255, 227);
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
            tableLayoutPanelHeader.Size = new Size(255, 55);
            tableLayoutPanelHeader.TabIndex = 0;
            // 
            // Title
            // 
            Title.Anchor = AnchorStyles.None;
            Title.AutoEllipsis = true;
            Title.AutoSize = true;
            Title.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            Title.ForeColor = Color.Black;
            Title.Location = new Point(61, 12);
            Title.Name = "Title";
            Title.Size = new Size(132, 30);
            Title.TabIndex = 0;
            Title.Text = "Комната \"\"";
            // 
            // connectButton
            // 
            connectButton.BackColor = Color.FromArgb(167, 157, 255);
            connectButton.Cursor = Cursors.Hand;
            connectButton.Dock = DockStyle.Bottom;
            connectButton.FlatAppearance.BorderColor = Color.FromArgb(228, 230, 240);
            connectButton.FlatStyle = FlatStyle.Flat;
            connectButton.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 204);
            connectButton.ForeColor = Color.FromArgb(248, 249, 255);
            connectButton.Location = new Point(0, 124);
            connectButton.Name = "connectButton";
            connectButton.Padding = new Padding(8, 4, 8, 4);
            connectButton.Size = new Size(255, 44);
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
            tableLayoutPanelLabels.Controls.Add(createdAt, 1, 1);
            tableLayoutPanelLabels.Controls.Add(labelCreatedAt, 0, 1);
            tableLayoutPanelLabels.Controls.Add(creatorLabel, 0, 0);
            tableLayoutPanelLabels.Controls.Add(hostName, 1, 0);
            tableLayoutPanelLabels.Controls.Add(currentlyConnectedLabel, 0, 4);
            tableLayoutPanelLabels.Controls.Add(participantsInfo, 1, 4);
            tableLayoutPanelLabels.Controls.Add(computerNumberLabel, 0, 3);
            tableLayoutPanelLabels.Controls.Add(computer, 1, 3);
            tableLayoutPanelLabels.Controls.Add(roomNumberLabel, 0, 2);
            tableLayoutPanelLabels.Controls.Add(classroom, 1, 2);
            tableLayoutPanelLabels.Dock = DockStyle.Top;
            tableLayoutPanelLabels.Location = new Point(0, 0);
            tableLayoutPanelLabels.Margin = new Padding(0);
            tableLayoutPanelLabels.Name = "tableLayoutPanelLabels";
            tableLayoutPanelLabels.RowCount = 5;
            tableLayoutPanelLabels.RowStyles.Add(new RowStyle(SizeType.Percent, 20.0004959F));
            tableLayoutPanelLabels.RowStyles.Add(new RowStyle(SizeType.Percent, 20.0005016F));
            tableLayoutPanelLabels.RowStyles.Add(new RowStyle(SizeType.Percent, 20.0004978F));
            tableLayoutPanelLabels.RowStyles.Add(new RowStyle(SizeType.Percent, 19.9985046F));
            tableLayoutPanelLabels.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanelLabels.Size = new Size(255, 121);
            tableLayoutPanelLabels.TabIndex = 1;
            // 
            // createdAt
            // 
            createdAt.Anchor = AnchorStyles.Left;
            createdAt.AutoEllipsis = true;
            createdAt.AutoSize = true;
            createdAt.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            createdAt.ForeColor = Color.FromArgb(0, 0, 0);
            createdAt.Location = new Point(130, 27);
            createdAt.Name = "createdAt";
            createdAt.Size = new Size(74, 17);
            createdAt.TabIndex = 13;
            createdAt.Text = "Загрузка...";
            // 
            // labelCreatedAt
            // 
            labelCreatedAt.Anchor = AnchorStyles.Right;
            labelCreatedAt.AutoSize = true;
            labelCreatedAt.Font = new Font("Segoe UI", 8.25F);
            labelCreatedAt.ForeColor = Color.Black;
            labelCreatedAt.Location = new Point(70, 29);
            labelCreatedAt.Name = "labelCreatedAt";
            labelCreatedAt.Size = new Size(54, 13);
            labelCreatedAt.TabIndex = 12;
            labelCreatedAt.Text = "Создана:";
            // 
            // creatorLabel
            // 
            creatorLabel.Anchor = AnchorStyles.Right;
            creatorLabel.AutoSize = true;
            creatorLabel.Font = new Font("Segoe UI", 8.25F);
            creatorLabel.ForeColor = Color.Black;
            creatorLabel.Location = new Point(60, 5);
            creatorLabel.Name = "creatorLabel";
            creatorLabel.Size = new Size(64, 13);
            creatorLabel.TabIndex = 11;
            creatorLabel.Text = "Создатель:";
            // 
            // hostName
            // 
            hostName.Anchor = AnchorStyles.Left;
            hostName.AutoEllipsis = true;
            hostName.AutoSize = true;
            hostName.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            hostName.ForeColor = Color.FromArgb(0, 0, 0);
            hostName.Location = new Point(130, 3);
            hostName.Name = "hostName";
            hostName.Size = new Size(74, 17);
            hostName.TabIndex = 1;
            hostName.Text = "Загрузка...";
            // 
            // currentlyConnectedLabel
            // 
            currentlyConnectedLabel.Anchor = AnchorStyles.Right;
            currentlyConnectedLabel.AutoSize = true;
            currentlyConnectedLabel.Font = new Font("Segoe UI", 8.25F);
            currentlyConnectedLabel.ForeColor = Color.Black;
            currentlyConnectedLabel.Location = new Point(45, 102);
            currentlyConnectedLabel.Name = "currentlyConnectedLabel";
            currentlyConnectedLabel.Size = new Size(79, 13);
            currentlyConnectedLabel.TabIndex = 9;
            currentlyConnectedLabel.Text = "Подключено:";
            // 
            // participantsInfo
            // 
            participantsInfo.Anchor = AnchorStyles.Left;
            participantsInfo.AutoSize = true;
            participantsInfo.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            participantsInfo.ForeColor = Color.FromArgb(0, 0, 0);
            participantsInfo.Location = new Point(130, 100);
            participantsInfo.Name = "participantsInfo";
            participantsInfo.Size = new Size(74, 17);
            participantsInfo.TabIndex = 7;
            participantsInfo.Text = "Загрузка...";
            // 
            // computerNumberLabel
            // 
            computerNumberLabel.Anchor = AnchorStyles.Right;
            computerNumberLabel.AutoSize = true;
            computerNumberLabel.Font = new Font("Segoe UI", 8.25F);
            computerNumberLabel.ForeColor = Color.Black;
            computerNumberLabel.Location = new Point(65, 77);
            computerNumberLabel.Name = "computerNumberLabel";
            computerNumberLabel.Size = new Size(59, 13);
            computerNumberLabel.TabIndex = 8;
            computerNumberLabel.Text = "№ Компа:";
            // 
            // computer
            // 
            computer.Anchor = AnchorStyles.Left;
            computer.AutoSize = true;
            computer.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            computer.ForeColor = Color.FromArgb(0, 0, 0);
            computer.Location = new Point(130, 75);
            computer.Name = "computer";
            computer.Size = new Size(74, 17);
            computer.TabIndex = 5;
            computer.Text = "Загрузка...";
            // 
            // roomNumberLabel
            // 
            roomNumberLabel.Anchor = AnchorStyles.Right;
            roomNumberLabel.AutoSize = true;
            roomNumberLabel.Font = new Font("Segoe UI", 8.25F);
            roomNumberLabel.ForeColor = Color.Black;
            roomNumberLabel.Location = new Point(70, 53);
            roomNumberLabel.Name = "roomNumberLabel";
            roomNumberLabel.Size = new Size(54, 13);
            roomNumberLabel.TabIndex = 10;
            roomNumberLabel.Text = "Кабинет:";
            // 
            // classroom
            // 
            classroom.Anchor = AnchorStyles.Left;
            classroom.AutoSize = true;
            classroom.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            classroom.ForeColor = Color.FromArgb(0, 0, 0);
            classroom.Location = new Point(130, 51);
            classroom.Name = "classroom";
            classroom.Size = new Size(74, 17);
            classroom.TabIndex = 4;
            classroom.Text = "Загрузка...";
            // 
            // RoomCard
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(splitContainer);
            Margin = new Padding(5);
            Name = "RoomCard";
            Size = new Size(255, 227);
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
        private Labels.Heading3Label computer;
        private Labels.Heading3Label classroom;
        private Labels.Heading3Label hostName;
        private Labels.Heading3Label participantsInfo;
        private Labels.CaptionLabel creatorLabel;
        private Labels.CaptionLabel roomNumberLabel;
        private Labels.CaptionLabel currentlyConnectedLabel;
        private Labels.CaptionLabel computerNumberLabel;
        private Labels.Heading3Label createdAt;
        private Labels.CaptionLabel labelCreatedAt;
    }
}
