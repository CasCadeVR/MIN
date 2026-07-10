using System;
using System.Threading.Tasks;
using Avalonia.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MIN.Core.Entities;
using MIN.Core.Entities.Contracts.Models;
using MIN.Desktop.Contracts.Constants;
using MIN.Desktop.Contracts.Enums;
using MIN.Desktop.Infrastructure.Services;
using MIN.Desktop.ViewModels.Base;
using MIN.Desktop.ViewModels.Cards;
using MIN.DI.FeatureCollection;

namespace MIN.Desktop.ViewModels.Pages;

/// <summary>
/// Модель боковой панели чата
/// </summary>
public partial class ChatSideBarViewModel : RoutableViewModelBase
{
    private IMinFeatureCollection featureCollection;
    private ParticipantInfo localParticipant = null!;
    private Guid roomId;

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
    public ChatSideBarViewModel(IMinFeatureCollection featureCollection)
    {
        this.featureCollection = featureCollection;

        OnNavigatedTo = (sender, e) => IsOpened = true;
        OnNavigatedFrom = (sender, e) =>
        {
            if (sender is ChatViewModel || sender is ChatSideBarViewModel)
            {
                IsOpened = false;
            }
        };
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

            //card.OnPrivateChatMenuStripClicked += (selected, particpant) =>
            //{
            //    foreach (var participantsFlowControl in participantsFlow.Controls)
            //    {
            //        if (participantsFlowControl is ParticipantCard participantCard)
            //        {
            //            if (participantCard.ParticipantId != participant.Id)
            //            {
            //                participantCard.Unselect();
            //            }
            //        }
            //    }

            //    privateChatParticipantId = selected ? participant.Id : null;
            //};

            //card.OnKickParticipantClicked += async (participant) =>
            //{
            //    var kickForm = new ParticipantKickForm(participant.Name);
            //    if (kickForm.ShowDialog() == DialogResult.OK)
            //    {
            //        try
            //        {
            //            await featureCollection.Chat.ChatRoomService.KickParticipantAsync(roomId,
            //            participant.Id, kickForm.Reason, formCts.Token);
            //        }
            //        catch (Exception ex)
            //        {
            //            MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //        }
            //    }
            //};

            RoomParticipants.Add(card);
        }
        //participantsInfo.Text = $"{room.ParticipantCount}/{room.MaximumParticipants}";
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
