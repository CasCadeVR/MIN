using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MIN.Common.Core.Contracts.Interfaces;
using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Events.Contracts;
using MIN.Core.Events.Events;
using MIN.Core.Stores.Contracts.Models;
using MIN.Desktop.Infrastructure.Events;
using MIN.Desktop.ViewModels.Base;

namespace MIN.Desktop.ViewModels.Pages;

/// <summary>
/// Модель карточки комнаты на боковой панели
/// </summary>
public partial class RecentRoomCardViewModel : CardViewModelBase, IDisposable
{
    private readonly IEventBus eventBus;
    private readonly RoomContext roomContext;
    private readonly RoomInfo roomInfo;

    private HashSet<IDisposable> eventTokens = null!;
    private int currentAmount;
    private int maximumAmount;

    /// <summary>
    /// Имя комнаты
    /// </summary>
    [ObservableProperty]
    public partial string RoomName { get; set; }

    /// <summary>
    /// Информация об подключенных участниках в формете (подключено)/(максимум)
    /// </summary>
    [ObservableProperty]
    public partial string ParticipantsInfo { get; set; } = string.Empty;

    /// <summary>
    /// Последнее сообщение
    /// </summary>
    [ObservableProperty]
    public partial string LastMessageContent { get; set; } = string.Empty;

    /// <summary>
    /// Количество пропущенных сообщений
    /// </summary>
    [ObservableProperty]
    public partial int MissedMessagesCount { get; set; }

    /// <summary>
    /// Время получения последнего сообщенния
    /// </summary>
    [ObservableProperty]
    public partial DateTime LastMessageReceivedAt { get; set; } = DateTime.Now;

    /// <summary>
    /// Выбрана ли карточка
    /// </summary>
    [ObservableProperty]
    public partial bool IsSelected { get; set; }

    /// <summary>
    /// Событие по нажатию на саму карточку
    /// </summary>
    public Action? Clicked { get; set; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="RecentRoomCardViewModel"/>
    /// </summary>
    public RecentRoomCardViewModel(IEventBus eventBus,
        RoomContext roomContext,
        RoomInfo roomInfo,
        bool AsCreator)
    {
        this.eventBus = eventBus;
        this.roomContext = roomContext;
        this.roomInfo = roomInfo;

        RoomName = roomInfo.Name;
        currentAmount = roomInfo.ParticipantCount + (AsCreator ? 1 : 0);
        maximumAmount = roomInfo.MaximumParticipants;

        GetLastMessage();
        UpdateParticipantsInfo();
        SubscribeToEvents();
    }

    private void GetLastMessage()
    {
        var lastMessage = roomContext.Messages.GetLastMessage();
        if (lastMessage is IDescribable describable)
        {
            LastMessageContent = describable.GetDescription();
        }
    }

    /// <summary>
    /// Выбрать карточку
    /// </summary>
    public void SelectCard()
    {
        IsSelected = true;
        MissedMessagesCount = 0;
    }

    /// <summary>
    /// Выбрать карточку
    /// </summary>
    [RelayCommand]
    public void SelectItem()
    {
        Clicked?.Invoke();
    }

    private void SubscribeToEvents()
    {
        eventTokens =
        [
            eventBus.Subscribe<ParticipantJoinedEvent>(OnParticipantJoined),
            eventBus.Subscribe<ParticipantLeftEvent>(OnParticipantLeft),
            eventBus.Subscribe<RoomInfoUpdatedMessageEvent>(OnRoomInfoUpdatedMessageEvent),
            eventBus.Subscribe<DescribableMessageReceivedEvent>(OnDescribableMessageReceivedEvent),
            eventBus.Subscribe<RoomClosedEvent>(OnRoomLeft),
        ];
    }

    private async Task OnParticipantJoined(ParticipantJoinedEvent eventMessage, CancellationToken cancellationToken)
    {
        if (eventMessage.RoomId != roomInfo.Id)
        {
            return;
        }

        currentAmount++;

        UpdateParticipantsInfo();
        await Task.CompletedTask;
    }

    private async Task OnParticipantLeft(ParticipantLeftEvent eventMessage, CancellationToken cancellationToken)
    {
        if (eventMessage.RoomId != roomInfo.Id)
        {
            return;
        }

        currentAmount--;

        UpdateParticipantsInfo();
        await Task.CompletedTask;
    }

    private Task OnRoomLeft(RoomClosedEvent eventMessage, CancellationToken cancellationToken)
    {
        if (eventMessage.RoomId != roomInfo.Id)
        {
            return Task.CompletedTask;
        }

        Dispose();
        return Task.CompletedTask;
    }

    private Task OnDescribableMessageReceivedEvent(DescribableMessageReceivedEvent eventMessage, CancellationToken cancellationToken)
    {
        if (eventMessage.RoomId != roomInfo.Id)
        {
            return Task.CompletedTask;
        }

        if (!IsSelected)
        {
            MissedMessagesCount++;
        }
        LastMessageReceivedAt = DateTime.Now;
        LastMessageContent = eventMessage.DescribableMessage.GetDescription();

        return Task.CompletedTask;
    }

    private async Task OnRoomInfoUpdatedMessageEvent(RoomInfoUpdatedMessageEvent eventMessage, CancellationToken ct)
    {
        if (eventMessage.RoomInfo.Id != roomInfo.Id)
        {
            return;
        }

        RoomName = eventMessage.RoomInfo.Name;
        roomInfo.Name = eventMessage.RoomInfo.Name;
        maximumAmount = eventMessage.RoomInfo.MaximumParticipants;

        UpdateParticipantsInfo();
        await Task.CompletedTask;
    }

    private void UpdateParticipantsInfo()
    {
        ParticipantsInfo = $"{currentAmount}/{maximumAmount}";
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
