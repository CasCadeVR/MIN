using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Collections;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MIN.Core.Entities;
using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Events.Contracts;
using MIN.Core.Events.Events;
using MIN.Desktop.Contracts.Constants;
using MIN.Desktop.Contracts.Enums;
using MIN.Desktop.Contracts.Interfaces;
using MIN.Desktop.Infrastructure.Events;
using MIN.Desktop.Infrastructure.Services;
using MIN.Desktop.ViewModels.Base;
using MIN.Desktop.ViewModels.Cards;
using MIN.Desktop.ViewModels.Modals;
using MIN.DI.FeatureCollection;

namespace MIN.Desktop.ViewModels.Pages;

/// <summary>
/// Модель боковой панели чата
/// </summary>
public partial class ChatSideBarViewModel : RoutableViewModelBase
{
    private readonly IMinFeatureCollection featureCollection;
    private readonly IDialogService dialogService;
    private readonly CancellationTokenSource appCts = null!;

    private HashSet<IDisposable> eventTokens = null!;
    private ParticipantInfo localParticipant = null!;
    private Guid roomId;

    private Guid? privateChatParticipantId;

    /// <inheritdoc />
    public override ViewLayoutType LayoutType => ViewLayoutType.RightSideBar;

    /// <inheritdoc />
    public override bool RelatedToCentral => true;

    /// <inheritdoc />
    public override EventHandler? OnNavigatedTo { get; }

    /// <inheritdoc />
    public override EventHandler? OnNavigatedFrom { get; }

    /// <summary>
    /// Участники комнаты
    /// </summary>
    [ObservableProperty]
    public partial AvaloniaList<ParticipantCardViewModel> RoomParticipants { get; set; } = [];

    /// <summary>
    /// Комната
    /// </summary>
    [ObservableProperty]
    public partial Room Room { get; set; } = null!;

    /// <summary>
    /// Имя хоста
    /// </summary>
    [ObservableProperty]
    public partial string HostName { get; set; } = string.Empty;

    /// <summary>
    /// IP Адрес
    /// </summary>
    [ObservableProperty]
    public partial string IpAddress { get; set; } = string.Empty;

    /// <summary>
    /// Порт
    /// </summary>
    [ObservableProperty]
    public partial string Port { get; set; } = string.Empty;

    /// <summary>
    /// Кабинет
    /// </summary>
    [ObservableProperty]
    public partial string Classroom { get; set; } = string.Empty;

    /// <summary>
    /// Информация о кол-ве учатсников
    /// </summary>
    [ObservableProperty]
    public partial string ParticipantsInfo { get; set; } = string.Empty;

    /// <summary>
    /// Являяется локальный пользователь хостом
    /// </summary>
    [ObservableProperty]
    public partial bool IsHost { get; set; }

    /// <summary>
    /// Открыта ли панель
    /// </summary>
    public bool IsOpened { get; set; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="ChatSideBarViewModel"/>
    /// </summary>
    public ChatSideBarViewModel(IMinFeatureCollection featureCollection,
        IDialogService dialogService,
        ICtsProvider ctsProvider)
    {
        this.featureCollection = featureCollection;
        this.dialogService = dialogService;

        if (!Design.IsDesignMode)
        {
            appCts = ctsProvider.AppCts;

            OnNavigatedTo = (sender, e) => IsOpened = true;
            OnNavigatedFrom = (sender, e) =>
            {
                if ((sender is ChatViewModel chatVm && chatVm.RoomId == roomId)
                    || (sender is ChatSideBarViewModel chatSideBarVm && chatSideBarVm.roomId == roomId))
                {
                    IsOpened = false;
                }
            };

            SubscribeToEvents(featureCollection.Core.EventBus);
        }
    }

    private void SubscribeToEvents(IEventBus eventBus)
    {
        eventTokens =
        [
            eventBus.Subscribe<ParticipantJoinedEvent>(OnParticipantJoined),
            eventBus.Subscribe<ParticipantLeftEvent>(OnParticipantLeft),
        ];
    }

    private async Task OnParticipantJoined(ParticipantJoinedEvent eventMessage, CancellationToken cancellationToken)
    {
        if (eventMessage.RoomId != roomId)
        {
            return;
        }

        Room.AddParticipant(eventMessage.Message.Participant);

        // TODO
        //AddMessageToChatFlow(eventMessage.Message);
        await featureCollection.Core.EventBus.PublishAsync(new DescribableMessageReceivedEvent()
        {
            RoomId = roomId,
            DescribableMessage = eventMessage.Message
        }, cancellationToken);
        //NotifyIfNeeded(eventMessage.Message);

        UpdateParticipantFlow();
    }

    private async Task OnParticipantLeft(ParticipantLeftEvent eventMessage, CancellationToken cancellationToken)
    {
        if (eventMessage.RoomId != roomId)
        {
            return;
        }

        var leavingParticipantId = eventMessage.Message.Participant.Id;
        Room.RemoveParticipantById(leavingParticipantId);
        if (privateChatParticipantId == leavingParticipantId)
        {
            privateChatParticipantId = null;
        }

        //AddMessageToChatFlow(eventMessage.Message);
        await featureCollection.Core.EventBus.PublishAsync(new DescribableMessageReceivedEvent()
        {
            RoomId = roomId,
            DescribableMessage = eventMessage.Message,
        }, cancellationToken);
        //NotifyIfNeeded(eventMessage.Message);

        UpdateParticipantFlow();
    }

    /// <summary>
    /// Подгрузить данные о комнате и перезагрузить страницу
    /// </summary>
    public async Task LoadRoomDataAndRefresh(Room room, ParticipantInfo localParticipant)
    {
        this.localParticipant = localParticipant;
        Room = room;
        roomId = room.Id;

        UpdateStats();
    }

    private void UpdateStats()
    {
        IsHost = Room.HostParticipant?.Id == localParticipant.Id;
        HostName = IsHost ? "Ты" : Room.HostParticipant?.Name ?? "Неизвестно";

        if (IpAddressParser.TryParseIpAddress(Room.ConnectionAddress, out var gottenIpAddress, out var port))
        {
            Port = port.ToString();
            IpAddress = gottenIpAddress;
        }

        Classroom = string.IsNullOrEmpty(Room.Cabinet) ? DesktopConstants.UndefinedPcName : Room.Cabinet;

        UpdateParticipantFlow();
    }

    private void UpdateParticipantFlow()
    {
        RoomParticipants.Clear();

        foreach (var participant in Room.CurrentParticipants)
        {
            var card = new ParticipantCardViewModel(participant,
                featureCollection.Core.EventBus,
                roomId,
                isHost: participant.Id == Room.HostParticipant.Id,
                isSelf: participant.Id == localParticipant.Id,
                asHost: localParticipant.Id == Room.HostParticipant.Id);

            card.OnPrivateChatMenuStripClicked += (selected, particpant) =>
            {
                foreach (var participantCard in RoomParticipants)
                {
                    if (participantCard.ParticipantId != participant.Id)
                    {
                        participantCard.IsSelected = false;
                    }
                }

                privateChatParticipantId = selected ? participant.Id : null;
            };

            card.OnKickParticipantClicked += async (participant) =>
            {
                var kickVm = await dialogService.ShowDialogAsync<ParticipantKickViewModel>(x => x.ParticipantName = participant.Name);

                if (kickVm! == true)
                {
                    try
                    {
                        await featureCollection.Chat.ChatRoomService.KickParticipantAsync(roomId,
                            participant.Id, kickVm!.Reason, appCts.Token);
                    }
                    catch (Exception ex)
                    {
                        InAppNotifier.Error(ex.Message);
                    }
                }
            };

            RoomParticipants.Add(card);
        }

        ParticipantsInfo = $"{Room.ParticipantCount}/{Room.MaximumParticipants}";
    }

    /// <summary>
    /// Закрыть страницу
    /// </summary>
    [RelayCommand]
    public void CloseAsync()
    {
        CloseView();
    }
}
