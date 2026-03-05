using System.Windows.Forms;
using MIN.Desktop.Contracts;
using MIN.Desktop.Contracts.Views.Forms;
using MIN.Services.Contracts.Interfaces;

namespace MIN.Desktop
{
    public partial class LogForm : StyledForm
    {
        private readonly ILoggerProvider loggerProvider;

        public LogForm(ILoggerProvider loggerProvider)
        {
            InitializeComponent();
            this.loggerProvider = loggerProvider;
            loggerProvider.OnLogReceived += OnLogReceived;
        }

        private void OnLogReceived(object? sender, string e)
        {
            AddLogMessage(e);
        }

        private void AddLogMessage(string message)
        {
            logListBox.Items.Add(message);
            int visibleItems = logListBox.ClientSize.Height / logListBox.ItemHeight;
            logListBox.TopIndex = Math.Max(logListBox.Items.Count - visibleItems + 1, 0);
        }

        protected override void ApplyStylings()
        {
            splitContainer.Panel1.BackColor = ColorScheme.PrimaryAccent;
            splitContainer.Panel2.BackColor = ColorScheme.MainPanelBackground;
            Title.ForeColor = ColorScheme.TextOnAccent;
        }

        private void LogForm_Load(object sender, EventArgs e)
        {
            logListBox.Items.Clear();

            var history = loggerProvider.GetLogHistory();

            foreach (var message in history)
            {
                AddLogMessage(message);
            }
        }

        private void LogForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            loggerProvider.OnLogReceived -= OnLogReceived;
        }
    }
}
