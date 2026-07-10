using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MIN.Chat.Events;
using MIN.Core.Entities;
using MIN.Core.Entities.Contracts.Enums;
using MIN.Core.Events.Contracts;
using MIN.Desktop.ViewModels.Base;

namespace MIN.Desktop.ViewModels.Cards;

/// <summary>
/// Модель карточки участника на боковой панели
/// </summary>
public partial class ParticipantCardViewModel : CardViewModelBase, IDisposable
{
    private const string StartPrivateChatText = "Начать приватное общение";
    private const string StopPrivateChatText = "Прекратить приватное общение";

    private readonly Participant participant;
    private readonly IEventBus eventBus;
    private readonly Guid roomId;
    private readonly bool isHost;
    private readonly bool isSelf;
    private HashSet<IDisposable> eventTokens = null!;

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
    /// Роль участника
    /// </summary>
    [ObservableProperty]
    public partial string ParticipantRole { get; set; } = string.Empty;

    /// <summary>
    /// Статус участника
    /// </summary>
    [ObservableProperty]
    public partial OnlineStatus ParticipantStatus { get; set; }

    /// <summary>
    /// Время последнего онлайн
    /// </summary>
    [ObservableProperty]
    public partial DateTime ParticipantLastSeenAt { get; set; } = DateTime.Now;

    /// <summary>
    /// Выбрана ли карточка
    /// </summary>
    [ObservableProperty]
    public partial bool IsSelected { get; set; }

    /// <summary>
    /// Является ли сейчас пользователь не в сети
    /// </summary>
    [ObservableProperty]
    public partial bool IsOffline { get; set; }

    /// <summary>
    /// Событие по нажатию на саму карточку
    /// </summary>
    public Action? Clicked { get; set; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="ParticipantCardViewModel"/>
    /// </summary>
    public ParticipantCardViewModel(Participant participant,
        IEventBus eventBus,
        Guid roomId,
        bool isHost,
        bool isSelf,
        bool asHost)
    {
        this.participant = participant;
        this.eventBus = eventBus;
        this.roomId = roomId;
        this.isHost = isHost;
        this.isSelf = isSelf;

        if (!Design.IsDesignMode)
        {
            FillLabels();

            if (!isSelf)
            {
                SubscribeToEvents();
            }
        }
    }

    private void SubscribeToEvents()
    {
        eventTokens =
        [
            eventBus.Subscribe<OnlineStatusChangedEvent>(OnOnlineStatusChanged),
        ];
    }

    private async Task OnOnlineStatusChanged(OnlineStatusChangedEvent eventMessage, CancellationToken cancellationToken)
    {
        if (eventMessage.RoomId != roomId)
        {
            return;
        }

        if (eventMessage.SenderId == participant.Id)
        {
            ParticipantStatus = isSelf ? OnlineStatus.Online : eventMessage.Status;
            IsOffline = ParticipantStatus == OnlineStatus.Offline;
        }
    }

    /// <summary>
    /// Отменить выбор карточки
    /// </summary>
    public void Unselect()
    {
        IsSelected = false;
        UpdateStylesOutOfSelected();
    }

    /// <summary>
    /// Выбрать карточку
    /// </summary>
    public void SelectCard()
    {
        IsSelected = true;
    }

    /// <summary>
    /// Выбрать карточку
    /// </summary>
    [RelayCommand]
    public void SelectItem()
    {
        Clicked?.Invoke();
    }

    //private void OnPrivateChatClickMenuStripClicked()
    //{
    //    selected = !selected;
    //    UpdateStylesOutOfSelected();
    //    OnPrivateChatMenuStripClicked?.Invoke(selected, participant);
    //}

    //private void OnKickParticipantClickMenuStripClicked()
    //{
    //    OnKickParticipantClicked?.Invoke(participant);
    //}

    private void UpdateStylesOutOfSelected()
    {
        //ContextMenuStrip?.Items[0].Text = IsSelected ? StopPrivateChatText : StartPrivateChatText;
        //BackColor = IsSelected
        //    ? ColorScheme.PrivateParticipantCardBackground
        //    : ColorScheme.DefaultParticipantCardBackground;
    }

    private void FillLabels()
    {
        ParticipantName = participant.Name;
        ParticipantStatus = isSelf ? OnlineStatus.Online : participant.CurrentStatus;
        IsOffline = ParticipantStatus == OnlineStatus.Offline;

        if (isHost)
        {
            ParticipantRole = "Хост";
        }
        else
        {
            ParticipantRole = "";
        }
    }

    /// <inheritdoc cref="IDisposable.Dispose"/>
    void IDisposable.Dispose()
    {
        foreach (var token in eventTokens)
        {
            token.Dispose();
        }
    }
}
