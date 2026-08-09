using System;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Events.Contracts.Interfaces;
using MIN.Voice.Events;
using MIN.Voice.Messaging;

namespace MIN.Desktop.ViewModels.Cards.Messages.Voice;

/// <summary>
/// Сообщение звонка участника
/// </summary>
public partial class ChatVoiceCallMessageViewModel : BaseChatMessageViewModel
{
    /// <summary>
    /// Звонок отклонён
    /// </summary>
    [ObservableProperty]
    public partial bool AsRejected { get; set; }

    /// <summary>
    /// Звонок завершён
    /// </summary>
    [ObservableProperty]
    public partial bool AsEnded { get; set; }

    /// <summary>
    /// Событие, возникающее по нажатию на кнопку присоединиться
    /// </summary>
    public event Func<Task>? OnJoinRequested;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="ChatVoiceCallMessageViewModel"/>
    /// </summary>
    public ChatVoiceCallMessageViewModel(IEventScope roomScope,
        IEventBus eventBus,
        VoiceCallStartedMessage voiceCallStartedMessage,
        ParticipantInfo localParticipant,
        Thickness timePadding,
        bool isHostMessage,
        bool removeHeaders)
        : base(voiceCallStartedMessage.Sender.Name,
            voiceCallStartedMessage.Timestamp,
            timePadding,
            localParticipant.Id == voiceCallStartedMessage.SenderId,
            isHostMessage,
            removeHeaders,
            voiceCallStartedMessage.RecipientId != null)
    {
        AsEnded = voiceCallStartedMessage.IsEnded;
        SubscribeToEvents(roomScope, eventBus);
    }

    private void SubscribeToEvents(IEventScope roomScope, IEventBus eventBus)
    {
        roomScope.Subscribe<VoiceCallEndedEvent>(OnVoiceCallEnded);
    }

    private async Task OnVoiceCallEnded(VoiceCallEndedEvent eventMessage, CancellationToken cancellationToken)
    {
        AsEnded = true;
        await Task.CompletedTask;
    }

    [RelayCommand]
    private void JoinVoiceCall()
    {
        OnJoinRequested?.Invoke();
    }

    [RelayCommand]
    private void RejectVoiceCall()
    {
        AsRejected = true;
    }
}
