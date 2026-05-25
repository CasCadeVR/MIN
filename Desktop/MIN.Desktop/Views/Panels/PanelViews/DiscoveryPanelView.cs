using MIN.Core.Entities;
using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Stores.Contracts.Registries.Models;
using MIN.Core.Transport.Contracts.Interfaces;
using MIN.Desktop.Components;
using MIN.Desktop.Contracts.Constants;
using MIN.Desktop.Contracts.Interfaces;
using MIN.Desktop.Contracts.Schemes;
using MIN.Desktop.Contracts.Views.PanelViews;
using MIN.Desktop.Infrastructure.Events;
using MIN.Desktop.Infrastructure.Services;
using MIN.Desktop.Views.Forms.HelperForms;
using MIN.Desktop.Views.Panels.PanelViews.ChatPanel;
using MIN.DI.FeatureCollection;
using MIN.Discovery.Events;
using MIN.Helpers.Contracts.Extensions;
using MIN.Helpers.Contracts.Models;

namespace MIN.Desktop.Views.Panels.SidePanelViews;

/// <summary>
/// Панель обнаружения комнат
/// </summary>
public partial class DiscoveryPanelView : StyledPanelView
{
    private readonly IMinFeatureCollection featureCollection;
    private readonly INavigationService navigationService;
    private readonly IChatPanelManager chatPanelManager;
    private readonly ParticipantInfo localParticipant;
    private readonly CancellationTokenSource lifeTimeCts;

    private Settings Settings => featureCollection.Helper.SettingsProvider.GetSettings();
    private CancellationTokenSource? discoveryCts;
    private bool isDiscovering;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="DiscoveryPanelView"/>
    /// </summary>
    public DiscoveryPanelView(IMinFeatureCollection featureCollection,
        IChatPanelManager chatPanelManager,
        INavigationService navigationService)
    {
        InitializeComponent();

        this.featureCollection = featureCollection;
        this.chatPanelManager = chatPanelManager;
        this.navigationService = navigationService;

        localParticipant = featureCollection.Helper.IdentityService.SelfParticipant.ToParticipantInfo();

        lifeTimeCts = new CancellationTokenSource();

        SubscribeToEvents();
    }

    private void SubscribeToEvents()
    {
        featureCollection.Core.EventBus.Subscribe<RoomDiscoveredEvent>(OnRoomDiscovered);
    }

    private async void discoverRooms_Click(object sender, EventArgs e)
    {
        if (isDiscovering)
        {
            discoveryCts?.Cancel();
        }
        else
        {
            await PerformDiscovery();
        }
    }

    private async Task PerformDiscovery()
    {
        isDiscovering = true;

        uiContext.Post(_ =>
        {
            discoverRooms.Text = "Остановить поиск";
            splitContainerDiscoverRoom.Panel2Collapsed = false;
            flowLayoutPanelDiscoveredRooms.Controls.Clear();
            totalRoomsCount.Text = "Поиск комнат...";
        }, null);

        discoveryCts = CancellationTokenSource.CreateLinkedTokenSource(lifeTimeCts.Token);

        try
        {
            await featureCollection.Discovery.DiscoveryService.DiscoverRoomsAsync(
                TimeSpan.FromMilliseconds(Settings.DiscoveryTimeout), discoveryCts.Token);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Discovery failed: {ex.Message}", "Error");
        }
        finally
        {
            discoverRooms.Enabled = true;
            discoverRooms.Text = "Найти комнаты";
            splitContainerDiscoverRoom.Panel2Collapsed = true;
            var roomsCount = flowLayoutPanelDiscoveredRooms.Controls.Count;
            totalRoomsCount.Text = $"Всего нашлось комнат: {roomsCount}";
            isDiscovering = false;
        }
    }

    private Task OnRoomDiscovered(RoomDiscoveredEvent e, CancellationToken cancellationToken)
    {
        uiContext.Post(_ =>
        {
            foreach (var discoveryInfo in e.RoomDiscoveryInfos)
            {
                var card = new RoomDiscoveryCard(featureCollection.Core.EventBus, localParticipant,
                    discoveryInfo.Room)
                {
                    Parent = flowLayoutPanelDiscoveredRooms
                };

                card.Clicked += () => OnRoomJoin(discoveryInfo.Endpoint, discoveryInfo.Room);
                card.Disposed += (s, _) =>
                {
                    totalRoomsCount.Text = $"Всего нашлось комнат: {flowLayoutPanelDiscoveredRooms.Controls.Count}";
                };
            }

            var roomsCount = flowLayoutPanelDiscoveredRooms.Controls.Count;
            totalRoomsCount.Text = $"Всего нашлось комнат: {roomsCount}";
        }, null);

        return Task.CompletedTask;
    }

    private bool ResolveParticipant()
    {
        if (Settings.DefaultParticipantName != string.Empty)
        {
            localParticipant.Name = Settings.DefaultParticipantName;
            featureCollection.Helper.IdentityService.SetParticipant(localParticipant);
        }
        else
        {
            var participantCreateForm = new ParticipantCreateForm(featureCollection.Helper.IdentityService);
            if (participantCreateForm.ShowDialog() != DialogResult.OK)
            {
                return false;
            }
            Settings.DefaultParticipantName = featureCollection.Helper.IdentityService.SelfParticipant.Name;
            featureCollection.Helper.SettingsProvider.SaveSettings(Settings);
        }
        return true;
    }

    private async Task OnRoomJoin(IEndpoint endpoint, RoomInfo? roomInfo = null)
    {
        if (roomInfo != null && featureCollection.Core.RoomConnector.IsConnected(roomInfo.Id))
        {
            MessageBox.Show($"Вы уже подключены к этой комнате", "Ошибка",
                MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            return;
        }

        if (!ResolveParticipant())
        {
            return;
        }

        var connectCts = CancellationTokenSource.CreateLinkedTokenSource(lifeTimeCts.Token);
        LoadingForm? loadingForm = null;

        try
        {
            var connectionResult = await featureCollection.Core.RoomConnector.ConnectAsync(endpoint,
                DesktopConstants.RoomConnectionTimeoutMs, connectCts.Token);

            loadingForm = new LoadingForm(connectionResult.RoomId, featureCollection.Core.EventBus, async room =>
            {
                if (room == null)
                {
                    return;
                }
                var newRoomInfo = new RoomInfo(room);
                chatPanelManager.RegisterChat(newRoomInfo,
                    navigationService
                    .NavigateTo<ChatPanelView, (Room room, Guid connectionId)>((room, connectionResult.ConnectionId)));
                await featureCollection.Core.EventBus.PublishAsync(new RoomJoinedEvent()
                {
                    RoomId = room.Id,
                    RoomInfo = newRoomInfo,
                });
            }, connectCts, DesktopConstants.RoomConnectionTimeoutMs);
            loadingForm.Show();
        }
        catch (Exception ex)
        {
            loadingForm?.Close();
            MessageBox.Show($"Произошла ошибка: {ex.Message}, \nВозможно, комнаты уже и нет", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    /// <inheritdoc />
    protected override void ApplyStylings()
    {
        tableLayoutPanel.BackColor = ColorScheme.PrimaryAccent;
        statusStrip.BackColor = ColorScheme.SecondaryAccent;
        totalRoomsCount.ForeColor = ColorScheme.TextOnAccent;
        discoveryProgressBar.ForeColor = ColorScheme.PrimaryAccent;
    }

    private async void createRoom_Click(object sender, EventArgs e)
    {
        var roomCreateForm = new RoomCreateForm();

        if (roomCreateForm.ShowDialog() != DialogResult.OK)
        {
            return;
        }

        var roomInfo = roomCreateForm.Room;
        var roomId = roomInfo.Id;

        if (!ResolveParticipant())
        {
            return;
        }

        var localParticipant = featureCollection.Helper.IdentityService.SelfParticipant.ToParticipantInfo();
        var context = featureCollection.Core.RoomFactory.GetOrCreateContext(roomId);

        context.Connections.RegisterLocalParticipant(localParticipant);
        roomInfo.HostParticipant = localParticipant;

        var room = new Room(roomInfo);

        try
        {
            featureCollection.Core.RoomStore.Register(room);

            await featureCollection.Core.RoomHoster.StartHostingAsync(roomInfo, lifeTimeCts.Token);

            context.Participants.AddParticipant(new Participant(localParticipant));

            await featureCollection.Discovery.DiscoveryService.StartDiscoveryAsync(roomId, lifeTimeCts.Token);

            chatPanelManager.RegisterChat(roomInfo, navigationService.NavigateTo<ChatPanelView, (Room room, Guid connectionId)>(
                (featureCollection.Core.RoomStore.GetRoom(roomId),
                CoreRegistryConstants.LocalConnectionId)
            ));
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Создание комнаты прошло не успешно: {ex.Message}",
                "Ошибка",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    private async void connectDirectButton_Click(object sender, EventArgs e)
    {
        var directConnectForm = new TcpDirectConnectForm();

        if (directConnectForm.ShowDialog() != DialogResult.OK)
        {
            return;
        }

        await OnRoomJoin(directConnectForm.Endpoint);
    }
}
