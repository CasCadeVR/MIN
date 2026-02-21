using MIN.Desktop.Contracts;
using MIN.Desktop.Contracts.Views.Forms;
using MIN.Desktop.Infrastructure.Services;

namespace MIN.Desktop
{
    public partial class LogForm : StyledForm
    {
        public LogForm()
        {
            InitializeComponent();
        }

        protected override void ApplyStylings()
        {
            splitContainer.Panel1.BackColor = ColorScheme.PrimaryAccent;
            splitContainer.Panel2.BackColor = ColorScheme.MainPanelBackground;
            Title.ForeColor = ColorScheme.TextOnAccent;
        }

        private bool IsParticipantValid()
        {
            return !(string.IsNullOrEmpty(AppUserProvider.Instance.CurrentUser.Name));
        }

        private void createButton_Click(object sender, EventArgs e)
        {
            AppUserProvider.Instance.CurrentUser.Name = participantName.Text;

            if (!IsParticipantValid())
            {
                MessageBox.Show(
                    "Имя участника не может быть пустым",
                    "Ошибка валидации",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation
                );

                return;
            }

            DialogResult = DialogResult.OK;
        }
    }
}
