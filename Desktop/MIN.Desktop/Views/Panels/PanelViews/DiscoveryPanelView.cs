using MIN.Core.Entities;
using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Services.Contracts.Models;
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

                card.Clicked += () => OnRoomJoin(discoveryInfo.Endpoint, discoveryInfo.Room, card);
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

    private async Task OnRoomJoin(IEndpoint endpoint, RoomInfo? roomInfo = null, RoomDiscoveryCard? card = null)
    {
        if (roomInfo != null && featureCollection.Core.RoomConnector.IsConnected(roomInfo.Id))
        {
            MessageBox.Show($"Вы уже подключены к этой комнате", "Ошибка",
                MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            card?.EnableConnectButton();
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
            ConnectionResult connectionResult = new();

            loadingForm = new LoadingForm(featureCollection.Core.EventBus, async room =>
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

            connectionResult = await featureCollection.Core.RoomConnector.ConnectAsync(endpoint, connectCts.Token);

            loadingForm.RoomId = connectionResult.RoomId;
        }
        catch (Exception ex)
        {
            loadingForm?.Close();
            loadingForm?.Dispose();
            MessageBox.Show($"Произошла ошибка при подключении: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            card?.EnableConnectButton();
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

        try
        {
            await featureCollection.Core.RoomHoster.StartHostingAsync(roomInfo, roomCreateForm.WithPortForwarding, lifeTimeCts.Token);
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
        directConnectForm.Show();
        directConnectForm.OnConnect += async () =>
        {
            await OnRoomJoin(directConnectForm.Endpoint);
            directConnectForm.EnableConnectButton();
        };
    }
}
