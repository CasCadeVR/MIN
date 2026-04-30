using MIN.Desktop.Contracts.Schemes;
using MIN.Desktop.Contracts.Views.Forms;
using MIN.Helpers.Contracts.Extensions;
using MIN.Helpers.Contracts.Interfaces;

namespace MIN.Desktop.Views.Forms.HelperForms;

/// <summary>
/// ‘орма создани€ участника
/// </summary>
public partial class ParticipantCreateForm : StyledForm
{
    private readonly IIdentityService identityService;

    /// <summary>
    /// »нициализирует новый экземпл€р <see cref="ParticipantCreateForm"/>
    /// </summary>
    public ParticipantCreateForm(IIdentityService identityService)
    {
        InitializeComponent();
        this.identityService = identityService;
        Shown += (_, _) => participantName.Focus();
    }

    /// <inheritdoc />
    protected override void ApplyStylings()
    {
        splitContainer.Panel1.BackColor = ColorScheme.PrimaryAccent;
        splitContainer.Panel2.BackColor = ColorScheme.MainPanelBackground;
        Title.ForeColor = ColorScheme.TextOnAccent;
    }

    private bool IsParticipantValid()
        => !string.IsNullOrEmpty(participantName.Text);

    private void createButton_Click(object sender, EventArgs e)
    {
        if (!IsParticipantValid())
        {
            MessageBox.Show(
                "»м€ участника не может быть пустым",
                "ќшибка валидации",
                MessageBoxButtons.OK,
                MessageBoxIcon.Exclamation
            );

            return;
        }

        var newParticipant = identityService.SelfParticipant.ToParticipantInfo();
        newParticipant.Name = participantName.Text;

        identityService.SetParticipant(newParticipant);

        DialogResult = DialogResult.OK;
    }

    private void ParticipantCreateForm_Load(object sender, EventArgs e)
    {
        participantName.Focus();
    }
}
