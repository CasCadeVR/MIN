using MIN.Desktop.Contracts.Schemes;
using MIN.Desktop.Contracts.Views.Forms;

namespace MIN.Desktop.Views.Forms.HelperForms;

/// <summary>
/// Форма создания комнаты
/// </summary>
public partial class ParticipantKickForm : StyledForm
{
    private readonly string participantName;

    /// <summary>
    /// Причина
    /// </summary>
    public string Reason { get; set; } = string.Empty;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="ParticipantKickForm"/>
    /// </summary>
    public ParticipantKickForm(string participantName)
    {
        this.participantName = participantName;

        InitializeComponent();

        var title = $"Кик участника {participantName}";
        Title.Text = title;
        Text = "MIN - " + title;
    }

    /// <inheritdoc />
    protected override void ApplyStylings()
    {
        splitContainer.Panel1.BackColor = ColorScheme.PrimaryAccent;
        splitContainer.Panel2.BackColor = ColorScheme.MainPanelBackground;
        Title.ForeColor = ColorScheme.TextOnAccent;
    }

    private void createButton_Click(object sender, EventArgs e)
    {
        if (MessageBox.Show($"Вы точно хотите кикнуть участника \"{participantName}\"",
            "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
        {
            Reason = reasonTextBox.Text;
            DialogResult = DialogResult.OK;
        }
    }

    private void cancelButton_Click(object sender, EventArgs e)
    {
        DialogResult = DialogResult.Cancel;
    }
}
