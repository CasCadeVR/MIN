using MIN.Desktop.Contracts;
using MIN.Desktop.Contracts.Views.Forms;
using MIN.Helpers.Contracts.Interfaces;

namespace MIN.Desktop
{
    /// <summary>
    /// ‘орма логировани€
    /// </summary>
    public partial class LogForm : StyledForm
    {
        private readonly ILoggerProvider loggerProvider;
        private readonly SynchronizationContext uiContext;

        /// <summary>
        /// »нициализирует новый экземпл€р <see cref="LogForm"/>
        /// </summary>
        public LogForm(ILoggerProvider loggerProvider)
        {
            InitializeComponent();
            this.loggerProvider = loggerProvider;

            uiContext = SynchronizationContext.Current
                ?? throw new InvalidOperationException("Must be created on UI thread");

            loggerProvider.OnLogReceived += OnLogReceived;
        }

        private void OnLogReceived(object? sender, string e)
        {
            AddLogMessage(e);
        }

        private void AddLogMessage(string message)
        {
            uiContext.Post(_ =>
            {
                logListBox.Items.Add(message);
                var visibleItems = logListBox.ClientSize.Height / logListBox.ItemHeight;
                logListBox.TopIndex = Math.Max(logListBox.Items.Count - visibleItems + 1, 0);
            }, this);
        }

        /// <inheritdoc />
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
