using MIN.Desktop.Views.Components.ChatMessages;

namespace MIN.Desktop.Components
{
    partial class ChatTextMessageCard
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
            sendMessage = new MIN.Desktop.Components.Textboxes.ReadonlyTextbox();
            ContentPanel.SuspendLayout();
            SuspendLayout();
            // 
            // ContentPanel
            // 
            ContentPanel.Controls.Add(sendMessage);
            // 
            // sendMessage
            // 
            sendMessage.BackColor = SystemColors.Control;
            sendMessage.BorderStyle = BorderStyle.None;
            sendMessage.Dock = DockStyle.Fill;
            sendMessage.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            sendMessage.ForeColor = Color.Black;
            sendMessage.Location = new Point(3, 22);
            sendMessage.Margin = new Padding(3, 0, 0, 3);
            sendMessage.Multiline = true;
            sendMessage.Name = "sendMessage";
            sendMessage.ReadOnly = true;
            sendMessage.Size = new Size(276, 45);
            sendMessage.TabIndex = 5;
            sendMessage.Text = "Сообщение";
            // 
            // ChatTextMessageCard
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Margin = new Padding(0);
            Name = "ChatTextMessageCard";
            ContentPanel.ResumeLayout(false);
            ContentPanel.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Textboxes.ReadonlyTextbox sendMessage;
    }
}
