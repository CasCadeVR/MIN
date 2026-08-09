using System;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using MIN.Core.Entities;
using MIN.Core.Events.Contracts.Interfaces;
using MIN.Desktop.ViewModels.Base;
using MIN.Voice.Events;

namespace MIN.Desktop.ViewModels.Cards;

/// <summary>
/// Модель карточки участника в voice чате
/// </summary>
public partial class ParticipantVoiceCardViewModel : CardViewModelBase, IDisposable
{
    private readonly DispatcherTimer talkingTimer = new(TimeSpan.FromMilliseconds(500), DispatcherPriority.Background, Dispatcher.UIThread);

    private readonly Participant participant;

    /// <summary>
    /// Идентфикатор участника на карточке
    /// </summary>
    public Guid ParticipantId => participant.Id;

    /// <summary>
    /// Имя участника
    /// </summary>
    [ObservableProperty]
    public partial string ParticipantName { get; set; } = string.Empty;

    /// <summary>
    /// Заглушен ли микрофон у участника
    /// </summary>
    [ObservableProperty]
    public partial bool MicMuted { get; set; }

    /// <summary>
    /// Разговаривает ли сейчас участник
    /// </summary>
    [ObservableProperty]
    public partial bool Talking { get; set; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="ParticipantVoiceCardViewModel"/>
    /// </summary>
    public ParticipantVoiceCardViewModel(Participant participant,
        IEventScope roomScope)
    {
        this.participant = participant;

        if (!Design.IsDesignMode)
        {
            ParticipantName = participant.Name;
            SubscribeToEvents(roomScope);
            talkingTimer.Tick += OnTalkingTimerTick;
        }
    }

    private void OnTalkingTimerTick(object? sender, EventArgs e)
        => Talking = false;

    private void SubscribeToEvents(IEventScope roomScope)
    {
        roomScope.Subscribe<VoiceDataReceivedEvent>(OnVoiceDataReceived);
    }

    private async Task OnVoiceDataReceived(VoiceDataReceivedEvent eventMessage, CancellationToken cancellationToken)
    {
        if (eventMessage.ParticipantId == participant.Id)
        {
            Talking = true;
            talkingTimer.Start();
        }
    }
}
