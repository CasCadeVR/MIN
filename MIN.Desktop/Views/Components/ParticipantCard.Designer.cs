namespace MIN.Desktop.Components
{
    partial class ParticipantCard
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
            tableLayoutPanelLabels = new TableLayoutPanel();
            participantRole = new MIN.Desktop.Components.Labels.Heading3Label();
            lastOnline = new MIN.Desktop.Components.Labels.Heading3Label();
            participantName = new MIN.Desktop.Components.Labels.Heading3Label();
            tableLayoutPanelLabels.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanelLabels
            // 
            tableLayoutPanelLabels.ColumnCount = 2;
            tableLayoutPanelLabels.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanelLabels.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 64F));
            tableLayoutPanelLabels.Controls.Add(participantRole, 1, 0);
            tableLayoutPanelLabels.Controls.Add(lastOnline, 0, 1);
            tableLayoutPanelLabels.Controls.Add(participantName, 0, 0);
            tableLayoutPanelLabels.Dock = DockStyle.Fill;
            tableLayoutPanelLabels.Location = new Point(0, 0);
            tableLayoutPanelLabels.Margin = new Padding(0);
            tableLayoutPanelLabels.Name = "tableLayoutPanelLabels";
            tableLayoutPanelLabels.RowCount = 2;
            tableLayoutPanelLabels.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));
            tableLayoutPanelLabels.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanelLabels.Size = new Size(239, 40);
            tableLayoutPanelLabels.TabIndex = 2;
            // 
            // participantRole
            // 
            participantRole.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            participantRole.AutoSize = true;
            participantRole.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            participantRole.ForeColor = Color.FromArgb(0, 0, 0);
            participantRole.Location = new Point(197, 0);
            participantRole.Name = "participantRole";
            participantRole.Size = new Size(39, 17);
            participantRole.TabIndex = 3;
            participantRole.Text = "Роль";
            // 
            // lastOnline
            // 
            lastOnline.AutoSize = true;
            tableLayoutPanelLabels.SetColumnSpan(lastOnline, 2);
            lastOnline.Dock = DockStyle.Fill;
            lastOnline.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 204);
            lastOnline.ForeColor = Color.FromArgb(0, 0, 0);
            lastOnline.Location = new Point(3, 24);
            lastOnline.Name = "lastOnline";
            lastOnline.Size = new Size(233, 16);
            lastOnline.TabIndex = 2;
            lastOnline.Text = "Последний раз в сети";
            // 
            // participantName
            // 
            participantName.AutoSize = true;
            participantName.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            participantName.ForeColor = Color.FromArgb(0, 0, 0);
            participantName.Location = new Point(3, 0);
            participantName.Name = "participantName";
            participantName.Size = new Size(35, 17);
            participantName.TabIndex = 0;
            participantName.Text = "Имя";
            // 
            // ParticipantCard
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(tableLayoutPanelLabels);
            Margin = new Padding(5);
            MinimumSize = new Size(239, 40);
            Name = "ParticipantCard";
            Size = new Size(239, 40);
            tableLayoutPanelLabels.ResumeLayout(false);
            tableLayoutPanelLabels.PerformLayout();
            ResumeLayout(false);

        }

        #endregion

        private TableLayoutPanel tableLayoutPanelLabels;
        private Labels.Heading3Label lastOnline;
        private Labels.Heading3Label participantName;
        private Labels.Heading3Label participantRole;
    }
}
