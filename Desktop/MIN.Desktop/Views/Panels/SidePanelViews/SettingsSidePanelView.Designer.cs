namespace MIN.Desktop.Views.Panels.SidePanelViews
{
    partial class SettingsSidePanelView
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
            tableLayoutPanelHeader = new TableLayoutPanel();
            saveButton = new MIN.Desktop.Components.Controls.Buttons.CommonButton();
            Title = new MIN.Desktop.Components.Labels.Heading1Label();
            tableLayoutPanel1 = new TableLayoutPanel();
            captionLabel3 = new MIN.Desktop.Components.Labels.CaptionLabel();
            captionLabel2 = new MIN.Desktop.Components.Labels.CaptionLabel();
            captionLabel1 = new MIN.Desktop.Components.Labels.CaptionLabel();
            clearCacheLabel = new MIN.Desktop.Components.Labels.CaptionLabel();
            clearCacheButton = new MIN.Desktop.Components.Controls.Buttons.InvertedButton();
            discoveryPort = new MIN.Desktop.Components.Controls.NumericUpDowns.DefaultNumericUpDown();
            roomSearchTime = new MIN.Desktop.Components.Controls.NumericUpDowns.DefaultNumericUpDown();
            defaultName = new MIN.Desktop.Components.Controls.TextBoxes.DefaultTextBox();
            logButton = new MIN.Desktop.Components.Controls.Buttons.InvertedButton();
            logDescriptionLabel = new MIN.Desktop.Components.Labels.CaptionLabel();
            labelVersion = new MIN.Desktop.Components.Labels.CaptionLabel();
            ((System.ComponentModel.ISupportInitialize)splitContainer).BeginInit();
            splitContainer.Panel1.SuspendLayout();
            splitContainer.Panel2.SuspendLayout();
            splitContainer.SuspendLayout();
            tableLayoutPanelHeader.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)discoveryPort).BeginInit();
            ((System.ComponentModel.ISupportInitialize)roomSearchTime).BeginInit();
            SuspendLayout();
            // 
            // splitContainer
            // 
            splitContainer.BackColor = Color.Transparent;
            splitContainer.ForeColor = Color.FromArgb(45, 43, 58);
            // 
            // splitContainer.Panel1
            // 
            splitContainer.Panel1.Controls.Add(tableLayoutPanelHeader);
            // 
            // splitContainer.Panel2
            // 
            splitContainer.Panel2.Controls.Add(tableLayoutPanel1);
            splitContainer.Size = new Size(250, 622);
            // 
            // tableLayoutPanelHeader
            // 
            tableLayoutPanelHeader.ColumnCount = 2;
            tableLayoutPanelHeader.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 48F));
            tableLayoutPanelHeader.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanelHeader.Controls.Add(saveButton, 0, 0);
            tableLayoutPanelHeader.Controls.Add(Title, 1, 0);
            tableLayoutPanelHeader.Dock = DockStyle.Fill;
            tableLayoutPanelHeader.Location = new Point(0, 0);
            tableLayoutPanelHeader.Name = "tableLayoutPanelHeader";
            tableLayoutPanelHeader.RowCount = 1;
            tableLayoutPanelHeader.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanelHeader.Size = new Size(250, 48);
            tableLayoutPanelHeader.TabIndex = 1;
            // 
            // saveButton
            // 
            saveButton.Anchor = AnchorStyles.Right;
            saveButton.BackColor = Color.FromArgb(192, 192, 255);
            saveButton.FlatAppearance.BorderColor = Color.FromArgb(228, 230, 240);
            saveButton.FlatStyle = FlatStyle.Flat;
            saveButton.Font = new Font("Segoe UI Black", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 204);
            saveButton.ForeColor = Color.FromArgb(248, 249, 255);
            saveButton.Location = new Point(3, 3);
            saveButton.Name = "saveButton";
            saveButton.Size = new Size(42, 42);
            saveButton.TabIndex = 2;
            saveButton.Text = "<";
            saveButton.UseVisualStyleBackColor = false;
            saveButton.Click += saveButton_Click;
            // 
            // Title
            // 
            Title.Anchor = AnchorStyles.Left;
            Title.AutoSize = true;
            Title.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            Title.ForeColor = Color.Black;
            Title.Location = new Point(51, 9);
            Title.Name = "Title";
            Title.Size = new Size(130, 30);
            Title.TabIndex = 0;
            Title.Text = "Настройки";
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.BackColor = Color.Transparent;
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Controls.Add(captionLabel3, 0, 2);
            tableLayoutPanel1.Controls.Add(captionLabel2, 0, 1);
            tableLayoutPanel1.Controls.Add(captionLabel1, 0, 0);
            tableLayoutPanel1.Controls.Add(clearCacheLabel, 1, 4);
            tableLayoutPanel1.Controls.Add(clearCacheButton, 0, 4);
            tableLayoutPanel1.Controls.Add(discoveryPort, 1, 2);
            tableLayoutPanel1.Controls.Add(roomSearchTime, 1, 1);
            tableLayoutPanel1.Controls.Add(defaultName, 1, 0);
            tableLayoutPanel1.Controls.Add(logButton, 0, 3);
            tableLayoutPanel1.Controls.Add(logDescriptionLabel, 1, 3);
            tableLayoutPanel1.Controls.Add(labelVersion, 0, 6);
            tableLayoutPanel1.Dock = DockStyle.Top;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Margin = new Padding(0);
            tableLayoutPanel1.MaximumSize = new Size(0, 571);
            tableLayoutPanel1.MinimumSize = new Size(0, 571);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 7;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 72F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 64F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 64F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tableLayoutPanel1.Size = new Size(250, 571);
            tableLayoutPanel1.TabIndex = 2;
            // 
            // captionLabel3
            // 
            captionLabel3.Anchor = AnchorStyles.Right;
            captionLabel3.AutoSize = true;
            captionLabel3.Enabled = false;
            captionLabel3.Font = new Font("Segoe UI", 8.25F);
            captionLabel3.ForeColor = Color.Black;
            captionLabel3.Location = new Point(15, 82);
            captionLabel3.Name = "captionLabel3";
            captionLabel3.Size = new Size(107, 52);
            captionLabel3.TabIndex = 22;
            captionLabel3.Text = "UDP Порт комнат (Лучше не менять, по умолчанию 42069)";
            // 
            // captionLabel2
            // 
            captionLabel2.Anchor = AnchorStyles.Right;
            captionLabel2.AutoSize = true;
            captionLabel2.Enabled = false;
            captionLabel2.Font = new Font("Segoe UI", 8.25F);
            captionLabel2.ForeColor = Color.Black;
            captionLabel2.Location = new Point(38, 35);
            captionLabel2.Name = "captionLabel2";
            captionLabel2.Size = new Size(84, 26);
            captionLabel2.TabIndex = 21;
            captionLabel2.Text = "Время поиска комнат (в мс.)";
            // 
            // captionLabel1
            // 
            captionLabel1.Anchor = AnchorStyles.Right;
            captionLabel1.AutoSize = true;
            captionLabel1.Enabled = false;
            captionLabel1.Font = new Font("Segoe UI", 8.25F);
            captionLabel1.ForeColor = Color.Black;
            captionLabel1.Location = new Point(11, 5);
            captionLabel1.Name = "captionLabel1";
            captionLabel1.Size = new Size(111, 13);
            captionLabel1.TabIndex = 20;
            captionLabel1.Text = "Имя по умолчанию";
            // 
            // clearCacheLabel
            // 
            clearCacheLabel.Anchor = AnchorStyles.Left;
            clearCacheLabel.AutoSize = true;
            clearCacheLabel.Enabled = false;
            clearCacheLabel.Font = new Font("Segoe UI", 8.25F);
            clearCacheLabel.ForeColor = Color.Black;
            clearCacheLabel.Location = new Point(128, 220);
            clearCacheLabel.Name = "clearCacheLabel";
            clearCacheLabel.Size = new Size(116, 39);
            clearCacheLabel.TabIndex = 19;
            clearCacheLabel.Text = "Обычно нужен при неправильных настройках в кэше";
            // 
            // clearCacheButton
            // 
            clearCacheButton.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            clearCacheButton.BackColor = Color.FromArgb(248, 249, 255);
            clearCacheButton.FlatAppearance.BorderColor = Color.FromArgb(167, 157, 255);
            clearCacheButton.FlatAppearance.BorderSize = 2;
            clearCacheButton.FlatStyle = FlatStyle.Flat;
            clearCacheButton.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            clearCacheButton.ForeColor = Color.FromArgb(167, 157, 255);
            clearCacheButton.Location = new Point(3, 223);
            clearCacheButton.Margin = new Padding(3, 0, 0, 0);
            clearCacheButton.Name = "clearCacheButton";
            clearCacheButton.Size = new Size(122, 33);
            clearCacheButton.TabIndex = 18;
            clearCacheButton.Text = "Очистить кэш";
            clearCacheButton.UseVisualStyleBackColor = false;
            clearCacheButton.Click += clearCacheButton_Click;
            // 
            // discoveryPort
            // 
            discoveryPort.Anchor = AnchorStyles.Left;
            discoveryPort.BackColor = Color.White;
            discoveryPort.BorderStyle = BorderStyle.None;
            discoveryPort.Font = new Font("Segoe UI", 9.75F);
            discoveryPort.ForeColor = Color.FromArgb(122, 119, 143);
            discoveryPort.Location = new Point(128, 97);
            discoveryPort.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
            discoveryPort.Minimum = new decimal(new int[] { 50, 0, 0, 0 });
            discoveryPort.Name = "discoveryPort";
            discoveryPort.Size = new Size(56, 21);
            discoveryPort.TabIndex = 17;
            discoveryPort.Value = new decimal(new int[] { 42069, 0, 0, 0 });
            // 
            // roomSearchTime
            // 
            roomSearchTime.Anchor = AnchorStyles.Left;
            roomSearchTime.BackColor = Color.White;
            roomSearchTime.BorderStyle = BorderStyle.None;
            roomSearchTime.Font = new Font("Segoe UI", 9.75F);
            roomSearchTime.ForeColor = Color.FromArgb(122, 119, 143);
            roomSearchTime.Increment = new decimal(new int[] { 50, 0, 0, 0 });
            roomSearchTime.Location = new Point(128, 37);
            roomSearchTime.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            roomSearchTime.Minimum = new decimal(new int[] { 50, 0, 0, 0 });
            roomSearchTime.Name = "roomSearchTime";
            roomSearchTime.Size = new Size(56, 21);
            roomSearchTime.TabIndex = 1;
            roomSearchTime.Value = new decimal(new int[] { 1000, 0, 0, 0 });
            // 
            // defaultName
            // 
            defaultName.Anchor = AnchorStyles.Left;
            defaultName.BackColor = Color.White;
            defaultName.BorderStyle = BorderStyle.None;
            defaultName.Font = new Font("Segoe UI", 9.75F);
            defaultName.ForeColor = Color.FromArgb(122, 119, 143);
            defaultName.Location = new Point(128, 3);
            defaultName.Name = "defaultName";
            defaultName.Size = new Size(119, 18);
            defaultName.TabIndex = 15;
            // 
            // logButton
            // 
            logButton.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            logButton.BackColor = Color.FromArgb(248, 249, 255);
            logButton.FlatAppearance.BorderColor = Color.FromArgb(167, 157, 255);
            logButton.FlatAppearance.BorderSize = 2;
            logButton.FlatStyle = FlatStyle.Flat;
            logButton.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            logButton.ForeColor = Color.FromArgb(167, 157, 255);
            logButton.Location = new Point(3, 159);
            logButton.Margin = new Padding(3, 0, 0, 0);
            logButton.Name = "logButton";
            logButton.Size = new Size(122, 33);
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
            logDescriptionLabel.Location = new Point(128, 150);
            logDescriptionLabel.Name = "logDescriptionLabel";
            logDescriptionLabel.Size = new Size(112, 52);
            logDescriptionLabel.TabIndex = 11;
            logDescriptionLabel.Text = "Чисто для разраба, но можете и сами понаблюдать по приколу";
            // 
            // labelVersion
            // 
            labelVersion.Anchor = AnchorStyles.Bottom;
            labelVersion.AutoSize = true;
            tableLayoutPanel1.SetColumnSpan(labelVersion, 2);
            labelVersion.Enabled = false;
            labelVersion.Font = new Font("Segoe UI", 8.25F);
            labelVersion.ForeColor = Color.Black;
            labelVersion.Location = new Point(101, 558);
            labelVersion.Name = "labelVersion";
            labelVersion.Size = new Size(47, 13);
            labelVersion.TabIndex = 13;
            labelVersion.Text = "Версия:";
            // 
            // SettingsSidePanelView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            MinimumSize = new Size(250, 622);
            Name = "SettingsSidePanelView";
            Size = new Size(250, 622);
            Controls.SetChildIndex(splitContainer, 0);
            splitContainer.Panel1.ResumeLayout(false);
            splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer).EndInit();
            splitContainer.ResumeLayout(false);
            tableLayoutPanelHeader.ResumeLayout(false);
            tableLayoutPanelHeader.PerformLayout();
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)discoveryPort).EndInit();
            ((System.ComponentModel.ISupportInitialize)roomSearchTime).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanelHeader;
        private Desktop.Components.Labels.Heading1Label Title;
        private TableLayoutPanel tableLayoutPanel1;
        private Desktop.Components.Labels.CaptionLabel labelVersion;
        private Desktop.Components.Controls.Buttons.InvertedButton logButton;
        private Desktop.Components.Labels.CaptionLabel logDescriptionLabel;
        private Desktop.Components.Controls.NumericUpDowns.DefaultNumericUpDown roomSearchTime;
        private Desktop.Components.Controls.TextBoxes.DefaultTextBox defaultName;
        private Desktop.Components.Controls.Buttons.CommonButton saveButton;
        private Desktop.Components.Controls.NumericUpDowns.DefaultNumericUpDown discoveryPort;
        private Desktop.Components.Labels.CaptionLabel clearCacheLabel;
        private Desktop.Components.Controls.Buttons.InvertedButton clearCacheButton;
        private Desktop.Components.Labels.CaptionLabel captionLabel1;
        private Desktop.Components.Labels.CaptionLabel captionLabel3;
        private Desktop.Components.Labels.CaptionLabel captionLabel2;
    }
}
