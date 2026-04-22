using MIN.Desktop.Components.Controls.Buttons;

namespace MIN.Desktop
{
    partial class SettingsForm
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
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            var dataGridViewCellStyle1 = new DataGridViewCellStyle();
            splitContainer = new SplitContainer();
            tableLayoutPanelHeader = new TableLayoutPanel();
            Title = new MIN.Desktop.Components.Labels.Heading1Label();
            tableLayoutPanelButtons = new TableLayoutPanel();
            saveButton = new CommonButton();
            cancelButton = new InvertedButton();
            tableLayoutPanel1 = new TableLayoutPanel();
            labelVersion = new MIN.Desktop.Components.Labels.CaptionLabel();
            logButton = new InvertedButton();
            logDescriptionLabel = new MIN.Desktop.Components.Labels.CaptionLabel();
            pcNameDescription = new MIN.Desktop.Components.Labels.CaptionLabel();
            preferredPcNameList = new MIN.Desktop.Components.Controls.DGVs.OneColumnDataGridView();
            PCName = new DataGridViewTextBoxColumn();
            preferredSearch = new MIN.Desktop.Components.Controls.RadioButtons.DefaultRadioButton();
            preferredPcNameDescription = new MIN.Desktop.Components.Labels.CaptionLabel();
            classRoomSearch = new MIN.Desktop.Components.Controls.RadioButtons.DefaultRadioButton();
            classRoomDescription = new MIN.Desktop.Components.Labels.CaptionLabel();
            searchTypeDescription = new MIN.Desktop.Components.Labels.CaptionLabel();
            ClassTitleInput = new MIN.Desktop.Components.Labels.Heading3Label();
            roomSearchTime = new MIN.Desktop.Components.Controls.NumericUpDowns.DefaultNumericUpDown();
            heading3Label1 = new MIN.Desktop.Components.Labels.Heading3Label();
            heading3Label2 = new MIN.Desktop.Components.Labels.Heading3Label();
            defaultName = new MIN.Desktop.Components.Controls.TextBoxes.DefaultTextBox();
            ((System.ComponentModel.ISupportInitialize)splitContainer).BeginInit();
            splitContainer.Panel1.SuspendLayout();
            splitContainer.Panel2.SuspendLayout();
            splitContainer.SuspendLayout();
            tableLayoutPanelHeader.SuspendLayout();
            tableLayoutPanelButtons.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)preferredPcNameList).BeginInit();
            ((System.ComponentModel.ISupportInitialize)roomSearchTime).BeginInit();
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
            splitContainer.Size = new Size(416, 593);
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
            Title.Location = new Point(143, 12);
            Title.Name = "Title";
            Title.Size = new Size(130, 30);
            Title.TabIndex = 0;
            Title.Text = "Настройки";
            // 
            // tableLayoutPanelButtons
            // 
            tableLayoutPanelButtons.ColumnCount = 2;
            tableLayoutPanelButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanelButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanelButtons.Controls.Add(saveButton, 0, 0);
            tableLayoutPanelButtons.Controls.Add(cancelButton, 1, 0);
            tableLayoutPanelButtons.Dock = DockStyle.Bottom;
            tableLayoutPanelButtons.Location = new Point(0, 484);
            tableLayoutPanelButtons.Name = "tableLayoutPanelButtons";
            tableLayoutPanelButtons.RowCount = 1;
            tableLayoutPanelButtons.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanelButtons.Size = new Size(416, 50);
            tableLayoutPanelButtons.TabIndex = 2;
            // 
            // saveButton
            // 
            saveButton.Anchor = AnchorStyles.Right;
            saveButton.BackColor = Color.FromArgb(192, 192, 255);
            saveButton.FlatAppearance.BorderColor = Color.FromArgb(228, 230, 240);
            saveButton.FlatStyle = FlatStyle.Flat;
            saveButton.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 204);
            saveButton.ForeColor = Color.FromArgb(248, 249, 255);
            saveButton.Location = new Point(3, 3);
            saveButton.Name = "saveButton";
            saveButton.Padding = new Padding(8, 4, 8, 4);
            saveButton.Size = new Size(202, 44);
            saveButton.TabIndex = 2;
            saveButton.Text = "Сохранить";
            saveButton.UseVisualStyleBackColor = false;
            saveButton.Click += saveButton_Click;
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
            tableLayoutPanel1.Controls.Add(labelVersion, 0, 8);
            tableLayoutPanel1.Controls.Add(logButton, 0, 7);
            tableLayoutPanel1.Controls.Add(logDescriptionLabel, 1, 7);
            tableLayoutPanel1.Controls.Add(pcNameDescription, 0, 5);
            tableLayoutPanel1.Controls.Add(preferredPcNameList, 1, 5);
            tableLayoutPanel1.Controls.Add(preferredSearch, 0, 4);
            tableLayoutPanel1.Controls.Add(preferredPcNameDescription, 1, 4);
            tableLayoutPanel1.Controls.Add(classRoomSearch, 0, 3);
            tableLayoutPanel1.Controls.Add(classRoomDescription, 1, 3);
            tableLayoutPanel1.Controls.Add(searchTypeDescription, 1, 2);
            tableLayoutPanel1.Controls.Add(ClassTitleInput, 0, 1);
            tableLayoutPanel1.Controls.Add(roomSearchTime, 1, 1);
            tableLayoutPanel1.Controls.Add(heading3Label1, 0, 2);
            tableLayoutPanel1.Controls.Add(heading3Label2, 0, 0);
            tableLayoutPanel1.Controls.Add(defaultName, 1, 0);
            tableLayoutPanel1.Dock = DockStyle.Top;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Margin = new Padding(0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 9;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 7.03335857F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 7.033357F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 9.50556F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 9.50556F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 13.8009281F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 9.20062F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 35.451313F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 8.469302F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tableLayoutPanel1.Size = new Size(416, 485);
            tableLayoutPanel1.TabIndex = 1;
            // 
            // labelVersion
            // 
            labelVersion.Anchor = AnchorStyles.None;
            labelVersion.AutoSize = true;
            tableLayoutPanel1.SetColumnSpan(labelVersion, 2);
            labelVersion.Enabled = false;
            labelVersion.Font = new Font("Segoe UI", 8.25F);
            labelVersion.ForeColor = Color.Black;
            labelVersion.Location = new Point(184, 466);
            labelVersion.Name = "labelVersion";
            labelVersion.Size = new Size(47, 13);
            labelVersion.TabIndex = 13;
            labelVersion.Text = "Версия:";
            // 
            // logButton
            // 
            logButton.Anchor = AnchorStyles.Right;
            logButton.BackColor = Color.FromArgb(248, 249, 255);
            logButton.FlatAppearance.BorderColor = Color.FromArgb(167, 157, 255);
            logButton.FlatAppearance.BorderSize = 2;
            logButton.FlatStyle = FlatStyle.Flat;
            logButton.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            logButton.ForeColor = Color.FromArgb(167, 157, 255);
            logButton.Location = new Point(3, 425);
            logButton.Name = "logButton";
            logButton.Size = new Size(202, 33);
            logButton.TabIndex = 12;
            logButton.Text = "Открыть окно логов";
            logButton.UseVisualStyleBackColor = false;
            logButton.Click += logButton_Click;
            // 
            // logDescriptionLabel
            // 
            logDescriptionLabel.Anchor = AnchorStyles.Left;
            logDescriptionLabel.AutoSize = true;
            logDescriptionLabel.Enabled = false;
            logDescriptionLabel.Font = new Font("Segoe UI", 8.25F);
            logDescriptionLabel.ForeColor = Color.Black;
            logDescriptionLabel.Location = new Point(211, 422);
            logDescriptionLabel.Name = "logDescriptionLabel";
            logDescriptionLabel.Size = new Size(202, 39);
            logDescriptionLabel.TabIndex = 11;
            logDescriptionLabel.Text = "Окно логов - чисто для разраба, но можете и сами понаблюдать по приколу";
            // 
            // pcNameDescription
            // 
            pcNameDescription.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            pcNameDescription.AutoSize = true;
            pcNameDescription.Enabled = false;
            pcNameDescription.Font = new Font("Segoe UI", 8.25F);
            pcNameDescription.ForeColor = Color.Black;
            pcNameDescription.Location = new Point(10, 216);
            pcNameDescription.Name = "pcNameDescription";
            tableLayoutPanel1.SetRowSpan(pcNameDescription, 2);
            pcNameDescription.Size = new Size(195, 104);
            pcNameDescription.TabIndex = 9;
            pcNameDescription.Text = resources.GetString("pcNameDescription.Text");
            // 
            // preferredPcNameList
            // 
            preferredPcNameList.AllowUserToResizeColumns = false;
            preferredPcNameList.AllowUserToResizeRows = false;
            preferredPcNameList.BackgroundColor = Color.FromArgb(255, 255, 255);
            preferredPcNameList.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            preferredPcNameList.Columns.AddRange(new DataGridViewColumn[] { PCName });
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = Color.FromArgb(106, 91, 255);
            dataGridViewCellStyle1.Font = new Font("Segoe UI", 8.25F);
            dataGridViewCellStyle1.ForeColor = Color.FromArgb(255, 255, 255);
            dataGridViewCellStyle1.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = DataGridViewTriState.False;
            preferredPcNameList.DefaultCellStyle = dataGridViewCellStyle1;
            preferredPcNameList.Dock = DockStyle.Fill;
            preferredPcNameList.Font = new Font("Segoe UI", 8.25F);
            preferredPcNameList.GridColor = Color.FromArgb(248, 249, 255);
            preferredPcNameList.Location = new Point(211, 219);
            preferredPcNameList.Name = "preferredPcNameList";
            preferredPcNameList.RowHeadersWidth = 51;
            tableLayoutPanel1.SetRowSpan(preferredPcNameList, 2);
            preferredPcNameList.Size = new Size(202, 200);
            preferredPcNameList.TabIndex = 10;
            preferredPcNameList.CellValidating += preferredPcNameList_CellValidating;
            // 
            // PCName
            // 
            PCName.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            PCName.HeaderText = "Компьютер";
            PCName.MinimumWidth = 6;
            PCName.Name = "PCName";
            // 
            // preferredSearch
            // 
            preferredSearch.Anchor = AnchorStyles.Right;
            preferredSearch.AutoSize = true;
            preferredSearch.BackColor = Color.FromArgb(248, 249, 255);
            preferredSearch.Font = new Font("Segoe UI", 9.75F);
            preferredSearch.ForeColor = Color.FromArgb(45, 43, 58);
            preferredSearch.Location = new Point(47, 173);
            preferredSearch.Name = "preferredSearch";
            preferredSearch.Size = new Size(158, 21);
            preferredSearch.TabIndex = 7;
            preferredSearch.Text = "Поиск по избранному";
            preferredSearch.UseVisualStyleBackColor = false;
            preferredSearch.CheckedChanged += preferredSearch_CheckedChanged;
            // 
            // preferredPcNameDescription
            // 
            preferredPcNameDescription.Anchor = AnchorStyles.Left;
            preferredPcNameDescription.AutoSize = true;
            preferredPcNameDescription.Enabled = false;
            preferredPcNameDescription.Font = new Font("Segoe UI", 8.25F);
            preferredPcNameDescription.ForeColor = Color.Black;
            preferredPcNameDescription.Location = new Point(211, 158);
            preferredPcNameDescription.Name = "preferredPcNameDescription";
            preferredPcNameDescription.Size = new Size(200, 52);
            preferredPcNameDescription.TabIndex = 6;
            preferredPcNameDescription.Text = "Вот это лучше - будет быстрее. Подходит если у тебя есть друзья за каким-то компом и ты знаешь к кому хочешь подключиться";
            // 
            // classRoomSearch
            // 
            classRoomSearch.Anchor = AnchorStyles.Right;
            classRoomSearch.AutoSize = true;
            classRoomSearch.BackColor = Color.FromArgb(248, 249, 255);
            classRoomSearch.Checked = true;
            classRoomSearch.Font = new Font("Segoe UI", 9.75F);
            classRoomSearch.ForeColor = Color.FromArgb(45, 43, 58);
            classRoomSearch.Location = new Point(67, 119);
            classRoomSearch.Name = "classRoomSearch";
            classRoomSearch.Size = new Size(138, 21);
            classRoomSearch.TabIndex = 3;
            classRoomSearch.TabStop = true;
            classRoomSearch.Text = "Поиск по кабинету";
            classRoomSearch.UseVisualStyleBackColor = false;
            classRoomSearch.CheckedChanged += classRoomSearch_CheckedChanged;
            // 
            // classRoomDescription
            // 
            classRoomDescription.Anchor = AnchorStyles.Left;
            classRoomDescription.AutoSize = true;
            classRoomDescription.Font = new Font("Segoe UI", 8.25F);
            classRoomDescription.ForeColor = Color.Black;
            classRoomDescription.Location = new Point(211, 110);
            classRoomDescription.Name = "classRoomDescription";
            classRoomDescription.Size = new Size(193, 39);
            classRoomDescription.TabIndex = 5;
            classRoomDescription.Text = "По умолчанию это. Оно проходит каждый компьютер, в  заданном кабинете (обычно их 20)\r\n";
            // 
            // searchTypeDescription
            // 
            searchTypeDescription.Anchor = AnchorStyles.Left;
            searchTypeDescription.AutoSize = true;
            searchTypeDescription.Font = new Font("Segoe UI", 8.25F);
            searchTypeDescription.ForeColor = Color.Black;
            searchTypeDescription.Location = new Point(211, 66);
            searchTypeDescription.Name = "searchTypeDescription";
            searchTypeDescription.Size = new Size(197, 39);
            searchTypeDescription.TabIndex = 4;
            searchTypeDescription.Text = "Ты можешь тут выбрать как искать комнаты, это повляет на скорость поиска";
            // 
            // ClassTitleInput
            // 
            ClassTitleInput.Anchor = AnchorStyles.Right;
            ClassTitleInput.AutoSize = true;
            ClassTitleInput.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 204);
            ClassTitleInput.ForeColor = Color.FromArgb(0, 0, 0);
            ClassTitleInput.Location = new Point(5, 39);
            ClassTitleInput.Name = "ClassTitleInput";
            ClassTitleInput.Size = new Size(200, 17);
            ClassTitleInput.TabIndex = 0;
            ClassTitleInput.Text = "Время поиска комнат (в мс.): ";
            // 
            // roomSearchTime
            // 
            roomSearchTime.Anchor = AnchorStyles.Left;
            roomSearchTime.BackColor = Color.White;
            roomSearchTime.BorderStyle = BorderStyle.None;
            roomSearchTime.Font = new Font("Segoe UI", 9.75F);
            roomSearchTime.ForeColor = Color.FromArgb(122, 119, 143);
            roomSearchTime.Increment = new decimal(new int[] { 100, 0, 0, 0 });
            roomSearchTime.Location = new Point(211, 37);
            roomSearchTime.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            roomSearchTime.Minimum = new decimal(new int[] { 100, 0, 0, 0 });
            roomSearchTime.Name = "roomSearchTime";
            roomSearchTime.Size = new Size(123, 21);
            roomSearchTime.TabIndex = 1;
            roomSearchTime.Value = new decimal(new int[] { 1000, 0, 0, 0 });
            // 
            // heading3Label1
            // 
            heading3Label1.Anchor = AnchorStyles.Right;
            heading3Label1.AutoSize = true;
            heading3Label1.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 204);
            heading3Label1.ForeColor = Color.FromArgb(0, 0, 0);
            heading3Label1.Location = new Point(68, 77);
            heading3Label1.Name = "heading3Label1";
            heading3Label1.Size = new Size(137, 17);
            heading3Label1.TabIndex = 2;
            heading3Label1.Text = "Тип поиска комнат: ";
            // 
            // heading3Label2
            // 
            heading3Label2.Anchor = AnchorStyles.Right;
            heading3Label2.AutoSize = true;
            heading3Label2.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 204);
            heading3Label2.ForeColor = Color.FromArgb(0, 0, 0);
            heading3Label2.Location = new Point(63, 7);
            heading3Label2.Name = "heading3Label2";
            heading3Label2.Size = new Size(142, 17);
            heading3Label2.TabIndex = 14;
            heading3Label2.Text = "Имя по умолчанию: ";
            // 
            // defaultName
            // 
            defaultName.Anchor = AnchorStyles.Left;
            defaultName.BackColor = Color.White;
            defaultName.BorderStyle = BorderStyle.None;
            defaultName.Font = new Font("Segoe UI", 9.75F);
            defaultName.ForeColor = Color.FromArgb(122, 119, 143);
            defaultName.Location = new Point(211, 7);
            defaultName.Name = "defaultName";
            defaultName.Size = new Size(197, 18);
            defaultName.TabIndex = 15;
            // 
            // SettingsForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(416, 593);
            Controls.Add(splitContainer);
            Margin = new Padding(3, 4, 3, 4);
            MinimumSize = new Size(432, 189);
            Name = "SettingsForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "MIN - Настройки";
            splitContainer.Panel1.ResumeLayout(false);
            splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer).EndInit();
            splitContainer.ResumeLayout(false);
            tableLayoutPanelHeader.ResumeLayout(false);
            tableLayoutPanelHeader.PerformLayout();
            tableLayoutPanelButtons.ResumeLayout(false);
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)preferredPcNameList).EndInit();
            ((System.ComponentModel.ISupportInitialize)roomSearchTime).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private SplitContainer splitContainer;
        private TableLayoutPanel tableLayoutPanelHeader;
        private TableLayoutPanel tableLayoutPanel1;
        private Components.Labels.Heading3Label ClassTitleInput;
        private Components.Labels.Heading1Label Title;
        private CommonButton saveButton;
        private TableLayoutPanel tableLayoutPanelButtons;
        private InvertedButton cancelButton;
        private Components.Labels.Heading3Label heading3Label1;
        private Components.Controls.NumericUpDowns.DefaultNumericUpDown roomSearchTime;
        private Components.Controls.RadioButtons.DefaultRadioButton classRoomSearch;
        private Components.Labels.CaptionLabel classRoomDescription;
        private Components.Labels.CaptionLabel searchTypeDescription;
        private Components.Controls.RadioButtons.DefaultRadioButton preferredSearch;
        private Components.Labels.CaptionLabel preferredPcNameDescription;
        private Components.Labels.CaptionLabel pcNameDescription;
        private Components.Controls.DGVs.OneColumnDataGridView preferredPcNameList;
        private DataGridViewTextBoxColumn PCName;
        private Components.Labels.CaptionLabel logDescriptionLabel;
        private InvertedButton logButton;
        private Components.Labels.CaptionLabel labelVersion;
        private Components.Labels.Heading3Label heading3Label2;
        private Components.Controls.TextBoxes.DefaultTextBox defaultName;
    }
}
