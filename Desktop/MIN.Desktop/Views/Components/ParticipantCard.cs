using MIN.Core.Entities.Contracts.Models;
using MIN.Desktop.Components.Controls.ContextMenuStrips;
using MIN.Desktop.Contracts;

namespace MIN.Desktop.Components;

/// <summary>
/// Карточка участника в комнате
/// </summary>
public partial class ParticipantCard : UserControl
{
    private const string StartPrivateChatText = "Начать приватное общение";
    private const string StopPrivateChatText = "Прекратить приватное общение";

    private readonly ParticipantInfo participant;
    private readonly bool isHost;
    private bool selected;

    /// <summary>
    /// Событие по нажатию ПКМ на карточку
    /// </summary>
    public Action<bool, ParticipantInfo>? OnCardContextMenuStripClicked { get; set; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="RoomCard"/>
    /// </summary>
    public ParticipantCard(ParticipantInfo participant, bool isHost, bool isSelf)
    {
        InitializeComponent();
        ApplyStylings();
        this.participant = participant;
        this.isHost = isHost;
        FillLabels();

        if (!isSelf)
        {
            var pictureBoxContextMenuStrip = new ParticipantCardContextMenuStrip();
            pictureBoxContextMenuStrip.OnItemClick += CardContextMenuStripClicked;
            pictureBoxContextMenuStrip.Items[0].Text = StartPrivateChatText;
            ContextMenuStrip = pictureBoxContextMenuStrip;
        }
    }

    private void CardContextMenuStripClicked()
    {
        selected = !selected;
        ContextMenuStrip!.Items[0].Text = selected ? StopPrivateChatText : StartPrivateChatText;
        BackColor = selected
            ? ColorScheme.PrivateParticipantCardBackground
            : ColorScheme.DefaultParticipantCardBackground;
        OnCardContextMenuStripClicked?.Invoke(selected, participant);
    }

    private void ApplyStylings()
    {
        participantName.Font = FontScheme.Caption;
        lastOnline.Font = FontScheme.MicroCaption;
        participantRole.Font = FontScheme.Caption;
        BackColor = ColorScheme.DefaultParticipantCardBackground;
    }

    private void FillLabels()
    {
        lastOnline.Text = string.Empty;
        participantName.Text = participant.Name;
        if (isHost)
        {
            participantRole.Text = "Хост";
        }
        else
        {
            participantRole.Text = "";
            tableLayoutPanelLabels.ColumnStyles[1].Width = 0;
        }
    }
}
