using MIN.Core.Entities;
using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Transport.Contracts.Interfaces;
using MIN.Desktop.Components;
using MIN.Desktop.Contracts.Constants;
using MIN.Desktop.Contracts.Interfaces;
using MIN.Desktop.Contracts.Schemes;
using MIN.Desktop.Contracts.Views.PanelViews;
using MIN.Desktop.Infrastructure.Events;
using MIN.Desktop.Views.Forms.HelperForms;
using MIN.Desktop.Views.Panels.PanelViews;
using MIN.DI;
using MIN.Discovery.Events;
using MIN.Helpers.Contracts.Extensions;
using MIN.Helpers.Contracts.Models;
using MIN.Helpers.Contracts.Models.Enums;
using MIN.Helpers.Services;

namespace MIN.Desktop.Views.Panels.SidePanelViews;

/// <summary>
/// Панель обнаружения комнат
/// </summary>
public partial class DiscoveryPanelView : StyledPanelView
{
    private readonly IMinFeatureCollection featureCollection;
    private readonly INavigationService navigationService;
    private readonly ParticipantInfo localParticipant;

    private Settings Settings => featureCollection.Helper.SettingsProvider.GetSettings();
    private CancellationTokenSource lifeTimeCts;
    private CancellationTokenSource? discoveryCts;
    private bool isDiscovering;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="DiscoveryPanelView"/>
    /// </summary>
    public DiscoveryPanelView(IMinFeatureCollection featureCollection,
        INavigationService navigationService)
    {
        InitializeComponent();
        ParseMachineName();

        this.featureCollection = featureCollection;
        this.navigationService = navigationService;

        localParticipant = featureCollection.Helper.IdentityService.SelfPartcipant.ToParticipantInfo();

        lifeTimeCts = new CancellationTokenSource();
        uiContext = SynchronizationContext.Current
            ?? throw new InvalidOperationException("");

        SubscribeToEvents();
    }

    private void ParseMachineName()
    {
        if (CollegePCNameParser.TryParseComputerName(featureCollection.Helper.ComputerProvider.GetLocalMachineName(), out var roomNumber, out var _))
        {
            classNumber.Value = roomNumber;
        }
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
            isDiscovering = false;
        }
        else
        {
            await PerformDiscovery();
        }
    }

    private async Task PerformDiscovery()
    {
        isDiscovering = true;

        var availablePCs = Settings.SearchMethod == SearchMethod.ClassRoom
                ? featureCollection.Helper.ComputerProvider.GetLocalNetworkMachineNames(classNumber.Value.ToString())
                : Settings.PreferredPCNames;

        uiContext.Post(_ =>
        {
            discoverRooms.Text = "Остановить поиск";
            splitContainerDiscoverRoom.Panel2Collapsed = false;
            discoveryProgressBar.Value = 1;
            discoveryProgressBar.Maximum = availablePCs.Count() + 1;
            flowLayoutPanelDiscoveredRooms.Controls.Clear();
            totalRoomsCount.Text = "Поиск комнат...";
        }, null);

        discoveryCts = CancellationTokenSource.CreateLinkedTokenSource(lifeTimeCts.Token);

        using var subscriptionToken = featureCollection.Core.EventBus.Subscribe((EndpointCheckedEvent _, CancellationToken _) =>
        {
            discoveryProgressBar.Value++;
            return Task.CompletedTask;
        });

        try
        {
            await featureCollection.Discovery.DiscoveryService.DiscoverRoomsAsync(availablePCs,
                TimeSpan.FromMilliseconds(Settings.DiscoveryTimeout), discoveryCts.Token);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Discovery failed: {ex.Message}", "Error");
        }
        finally
        {
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
                var card = new RoomCard(featureCollection.Core.EventBus, localParticipant,
                    discoveryInfo.Room, e.MachineName)
                {
                    Parent = flowLayoutPanelDiscoveredRooms
                };

                card.Clicked += () => OnRoomJoin(discoveryInfo.Room, discoveryInfo.Endpoint);
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
            Settings.DefaultParticipantName = featureCollection.Helper.IdentityService.SelfPartcipant.Name;
            featureCollection.Helper.SettingsProvider.SaveSettings(Settings);
        }
        return true;
    }

    private async Task OnRoomJoin(RoomInfo roomInfo, IEndpoint endpoint)
    {
        if (featureCollection.Core.RoomConnector.IsConnected(roomInfo.Id))
        {
            MessageBox.Show($"Вы уже подключены к этой комнате", "Ошибка",
                MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            return;
        }

        ResolveParticipant();

        try
        {
            featureCollection.Core.RoomStore.Add(new Room(roomInfo));

            var connectionId = Guid.Empty;
            var connectCts = CancellationTokenSource.CreateLinkedTokenSource(lifeTimeCts.Token);

            new LoadingForm(roomInfo.Id, featureCollection.Core.EventBus, async room =>
            {
                if (room == null)
                {
                    return;
                }
                navigationService.NavigateTo<ChatPanelView, (Guid roomId, Guid connectionId, IEndpoint endpoint)>((room.Id, connectionId, endpoint));
                await featureCollection.Core.EventBus.PublishAsync(new RoomJoinedEvent() { RoomId = room.Id });
            }, connectCts, DesktopConstants.RoomConnectionTimeoutMs).Show();

            featureCollection.Core.RoomFactory.GetOrCreateContext(roomInfo.Id)
                .Connections.RegisterLocalParticipant(localParticipant);

            connectionId = await featureCollection.Core.RoomConnector.ConnectAsync(roomInfo, endpoint,
                DesktopConstants.RoomConnectionTimeoutMs, connectCts.Token);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Произошла ошибка: {ex.Message}");
        }
    }

    /// <inheritdoc />
    protected override void ApplyStylings()
    {
        statusStrip.BackColor = ColorScheme.PrimaryAccent;
        totalRoomsCount.ForeColor = ColorScheme.TextOnAccent;
        discoveryProgressBar.ForeColor = ColorScheme.PrimaryAccent;
    }
}
