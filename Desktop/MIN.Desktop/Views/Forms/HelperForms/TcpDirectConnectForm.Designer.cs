using MIN.Desktop.Components.Controls.Buttons;
using MIN.Desktop.Components.Controls.NumericUpDowns;
using MIN.Desktop.Components.Controls.TextBoxes;
using MIN.Desktop.Components.Labels;

namespace MIN.Desktop.Views.Forms.HelperForms
{
    partial class TcpDirectConnectForm
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
            tableLayoutPanelButtons = new TableLayoutPanel();
            connectButton = new CommonButton();
            cancelButton = new InvertedButton();
            tableLayoutPanel1 = new TableLayoutPanel();
            portNumericUpDown = new DefaultNumericUpDown();
            portLabel = new Heading3Label();
            ipAddressLabel = new Heading3Label();
            ipAddress = new DefaultTextBox();
            ((System.ComponentModel.ISupportInitialize)splitContainer).BeginInit();
            splitContainer.Panel1.SuspendLayout();
            splitContainer.Panel2.SuspendLayout();
            splitContainer.SuspendLayout();
            tableLayoutPanelHeader.SuspendLayout();
            tableLayoutPanelButtons.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)portNumericUpDown).BeginInit();
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
            splitContainer.Size = new Size(363, 202);
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
            tableLayoutPanelHeader.Size = new Size(363, 55);
            tableLayoutPanelHeader.TabIndex = 0;
            // 
            // Title
            // 
            Title.Anchor = AnchorStyles.None;
            Title.AutoSize = true;
            Title.Font = new Font("Segoe UI", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 204);
            Title.ForeColor = Color.Black;
            Title.Location = new Point(44, 12);
            Title.Name = "Title";
            Title.Size = new Size(274, 30);
            Title.TabIndex = 0;
            Title.Text = "Подключение напрямую";
            // 
            // tableLayoutPanelButtons
            // 
            tableLayoutPanelButtons.ColumnCount = 2;
            tableLayoutPanelButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanelButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanelButtons.Controls.Add(connectButton, 0, 0);
            tableLayoutPanelButtons.Controls.Add(cancelButton, 1, 0);
            tableLayoutPanelButtons.Dock = DockStyle.Bottom;
            tableLayoutPanelButtons.Location = new Point(0, 93);
            tableLayoutPanelButtons.Name = "tableLayoutPanelButtons";
            tableLayoutPanelButtons.RowCount = 1;
            tableLayoutPanelButtons.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanelButtons.Size = new Size(363, 50);
            tableLayoutPanelButtons.TabIndex = 2;
            // 
            // connectButton
            // 
            connectButton.BackColor = Color.FromArgb(192, 192, 255);
            connectButton.Dock = DockStyle.Fill;
            connectButton.FlatAppearance.BorderColor = Color.FromArgb(228, 230, 240);
            connectButton.FlatStyle = FlatStyle.Flat;
            connectButton.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 204);
            connectButton.ForeColor = Color.FromArgb(248, 249, 255);
            connectButton.Location = new Point(3, 3);
            connectButton.Name = "connectButton";
            connectButton.Padding = new Padding(8, 4, 8, 4);
            connectButton.Size = new Size(175, 44);
            connectButton.TabIndex = 2;
            connectButton.Text = "Подключиться";
            connectButton.UseVisualStyleBackColor = false;
            connectButton.Click += connectButton_Click;
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
            cancelButton.Location = new Point(184, 3);
            cancelButton.Name = "cancelButton";
            cancelButton.Padding = new Padding(8, 4, 8, 4);
            cancelButton.Size = new Size(176, 44);
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
            tableLayoutPanel1.Controls.Add(portNumericUpDown, 1, 1);
            tableLayoutPanel1.Controls.Add(portLabel, 0, 1);
            tableLayoutPanel1.Controls.Add(ipAddressLabel, 0, 0);
            tableLayoutPanel1.Controls.Add(ipAddress, 1, 0);
            tableLayoutPanel1.Dock = DockStyle.Top;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Margin = new Padding(0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3333321F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3333321F));
            tableLayoutPanel1.Size = new Size(363, 94);
            tableLayoutPanel1.TabIndex = 1;
            // 
            // portNumericUpDown
            // 
            portNumericUpDown.Anchor = AnchorStyles.Left;
            portNumericUpDown.BackColor = Color.White;
            portNumericUpDown.BorderStyle = BorderStyle.None;
            portNumericUpDown.Font = new Font("Segoe UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            portNumericUpDown.ForeColor = Color.Purple;
            portNumericUpDown.Location = new Point(184, 56);
            portNumericUpDown.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
            portNumericUpDown.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            portNumericUpDown.Name = "portNumericUpDown";
            portNumericUpDown.Size = new Size(74, 29);
            portNumericUpDown.TabIndex = 1;
            portNumericUpDown.Value = new decimal(new int[] { 49152, 0, 0, 0 });
            // 
            // portLabel
            // 
            portLabel.Anchor = AnchorStyles.Right;
            portLabel.AutoSize = true;
            portLabel.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            portLabel.ForeColor = Color.FromArgb(0, 0, 0);
            portLabel.Location = new Point(10, 60);
            portLabel.Name = "portLabel";
            portLabel.Size = new Size(168, 21);
            portLabel.TabIndex = 0;
            portLabel.Text = "Порт подключения:";
            // 
            // ipAddressLabel
            // 
            ipAddressLabel.Anchor = AnchorStyles.Right;
            ipAddressLabel.AutoSize = true;
            ipAddressLabel.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            ipAddressLabel.ForeColor = Color.FromArgb(0, 0, 0);
            ipAddressLabel.Location = new Point(37, 13);
            ipAddressLabel.Name = "ipAddressLabel";
            ipAddressLabel.Size = new Size(141, 21);
            ipAddressLabel.TabIndex = 0;
            ipAddressLabel.Text = "Адрес комнаты: ";
            // 
            // ipAddress
            // 
            ipAddress.Anchor = AnchorStyles.Left;
            ipAddress.BackColor = Color.FromArgb(248, 249, 255);
            ipAddress.BorderStyle = BorderStyle.None;
            ipAddress.Font = new Font("Segoe UI", 14.25F);
            ipAddress.ForeColor = Color.Purple;
            ipAddress.Location = new Point(184, 10);
            ipAddress.Name = "ipAddress";
            ipAddress.Size = new Size(147, 26);
            ipAddress.TabIndex = 2;
            ipAddress.KeyPress += ipAddress_KeyPress;
            ipAddress.Leave += ipAddress_Leave;
            // 
            // TcpDirectConnectForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = cancelButton;
            ClientSize = new Size(363, 202);
            Controls.Add(splitContainer);
            MinimumSize = new Size(379, 241);
            Name = "TcpDirectConnectForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "MIN - Подключение напрямую";
            splitContainer.Panel1.ResumeLayout(false);
            splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer).EndInit();
            splitContainer.ResumeLayout(false);
            tableLayoutPanelHeader.ResumeLayout(false);
            tableLayoutPanelHeader.PerformLayout();
            tableLayoutPanelButtons.ResumeLayout(false);
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)portNumericUpDown).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private SplitContainer splitContainer;
        private TableLayoutPanel tableLayoutPanelHeader;
        private TableLayoutPanel tableLayoutPanel1;
        private Heading3Label ipAddressLabel;
        private Heading1Label Title;
        private CommonButton connectButton;
        private Heading3Label portLabel;
        private DefaultNumericUpDown portNumericUpDown;
        private TableLayoutPanel tableLayoutPanelButtons;
        private InvertedButton cancelButton;
        private DefaultTextBox ipAddress;
    }
}
