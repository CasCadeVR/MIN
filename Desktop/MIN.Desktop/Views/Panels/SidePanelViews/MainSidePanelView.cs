using MIN.Core.Entities;
using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Events.Events;
using MIN.Core.Stores.Contracts.Registries.Models;
using MIN.Core.Transport.Contracts.Interfaces;
using MIN.Core.Transport.NamedPipes.Models;
using MIN.Desktop.Components;
using MIN.Desktop.Contracts.Enums;
using MIN.Desktop.Contracts.Interfaces;
using MIN.Desktop.Contracts.Schemes;
using MIN.Desktop.Contracts.Views.PanelViews;
using MIN.Desktop.Infrastructure.Events;
using MIN.Desktop.Infrastructure.Services;
using MIN.Desktop.Views.Panels.PanelViews;
using MIN.DI.FeatureCollection;
using MIN.Helpers.Contracts.Extensions;

namespace MIN.Desktop.Views.Panels.SidePanelViews;

/// <summary>
/// Главная боковая панель
/// </summary>
public partial class MainSidePanelView : StyledPanelView, IChatPanelManager
{
    private readonly IMinFeatureCollection featureCollection;
    private readonly ICtsProvider ctsProvider;
    private readonly INavigationService navigationService;
    private readonly Dictionary<Guid, RecentRoomCard> activeRecentRoomCards = [];
    private readonly Dictionary<Guid, ChatPanelView> activeChatPanels = [];
    private Guid selectedChatPanelRoomId;

    /// <inheritdoc />
    public override PanelType PanelType => PanelType.Side;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="MainSidePanelView"/>
    /// </summary>
    public MainSidePanelView(IMinFeatureCollection featureCollection,
        ICtsProvider ctsProvider,
        INavigationService navigationService)
    {
        InitializeComponent();

        this.featureCollection = featureCollection;
        this.ctsProvider = ctsProvider;
        this.navigationService = navigationService;

        SubscribeToEvents();
    }

    private void SubscribeToEvents()
    {
        featureCollection.Core.EventBus.Subscribe<ErrorOccurredEvent>(OnErrorOccurredEvent);
        featureCollection.Core.EventBus.Subscribe<RoomClosedEvent>(OnRoomClosedEvent);
    }

    private Task OnErrorOccurredEvent(ErrorOccurredEvent e, CancellationToken cancellationToken)
    {
        uiContext.Post(_ =>
        {
            MessageBox.Show(e.ErrorMessage,
                "Ошибка",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }, null);

        return Task.CompletedTask;
    }

    private Task OnRoomClosedEvent(RoomClosedEvent e, CancellationToken cancellationToken)
    {
        UnregisterChat(e.RoomId);
        return Task.CompletedTask;
    }

    private bool ResolveParticipant()
    {
        var settings = featureCollection.Helper.SettingsProvider.GetSettings();
        var localParticipant = featureCollection.Helper.IdentityService.SelfPartcipant.ToParticipantInfo();

        if (settings.DefaultParticipantName != string.Empty)
        {
            localParticipant.Name = settings.DefaultParticipantName;
            featureCollection.Helper.IdentityService.SetParticipant(localParticipant);
        }
        else
        {
            var participantCreateForm = new ParticipantCreateForm(featureCollection.Helper.IdentityService);
            if (participantCreateForm.ShowDialog() != DialogResult.OK)
            {
                return false;
            }
            settings.DefaultParticipantName = featureCollection.Helper.IdentityService.SelfPartcipant.Name;
            featureCollection.Helper.SettingsProvider.SaveSettings(settings);
        }
        return true;
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

        var localParticipant = featureCollection.Helper.IdentityService.SelfPartcipant.ToParticipantInfo();
        var context = featureCollection.Core.RoomFactory.GetOrCreateContext(roomId);

        context.Connections.RegisterLocalParticipant(localParticipant);
        roomInfo.HostParticipant = localParticipant;

        var room = new Room(roomInfo);

        try
        {
            featureCollection.Core.RoomStore.Register(room);

            await featureCollection.Core.RoomHoster.StartHostingAsync(roomInfo, ctsProvider.AppCts.Token);

            context.Participants.AddParticipant(localParticipant);

            await featureCollection.Discovery.DiscoveryService.StartDiscoveryAsync(roomId, ctsProvider.AppCts.Token);

            RegisterChat(roomId, navigationService.NavigateTo<ChatPanelView, (Room room, Guid connectionId, IEndpoint endpoint)>(
                (featureCollection.Core.RoomStore.GetRoom(roomId),
                CoreRegistryConstants.LocalConnectionId,
                new NamedPipeEndpoint()
                {
                    MachineName = Environment.MachineName,
                })));
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Создание комнаты прошло не успешно: {ex.Message}",
                "Ошибка",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    /// <inheritdoc />
    public void RegisterChat(Guid roomId, ChatPanelView panel)
    {
        var context = featureCollection.Core.RoomFactory.GetOrCreateContext(roomId);

        activeChatPanels[roomId] = panel;
        var card = new RecentRoomCard(featureCollection.Core.EventBus, context,
            featureCollection.Core.RoomStore.GetRoom(roomId))
        {
            Width = flowLayoutPanelRooms.Width - flowLayoutPanelRooms.Margin.Horizontal * 2,
        };

        card.Clicked += () =>
        {
            if (selectedChatPanelRoomId != roomId)
            {
                SelectChatCard(roomId, card);
                navigationService.NavigateToExisting(panel);
            }
        };
        flowLayoutPanelRooms.Controls.Add(card);
        activeRecentRoomCards[roomId] = card;
        SelectChatCard(roomId, card);
    }

    private void UnselectAllChatCards()
    {
        foreach (var otherCard in activeRecentRoomCards.Values)
        {
            otherCard.BackColor = ColorScheme.Transparent;
        }

        selectedChatPanelRoomId = Guid.Empty;
    }

    private void SelectChatCard(Guid roomId, RecentRoomCard card)
    {
        UnselectAllChatCards();

        card.BackColor = ColorScheme.SecondaryAccent;
        selectedChatPanelRoomId = roomId;
    }

    /// <inheritdoc />
    public void UnregisterChat(Guid roomId)
    {
        activeChatPanels.Remove(roomId);

        if (activeRecentRoomCards.Remove(roomId, out var card))
        {
            card.Dispose();
        }
    }

    ChatPanelView? IChatPanelManager.GetChatPanel(Guid roomId)
        => activeChatPanels.TryGetValue(roomId, out var chatPanelView) ? chatPanelView : null;

    /// <inheritdoc />
    protected override void ApplyStylings()
    {
        BackColor = ColorScheme.MainPanelBackground;
    }

    private void settingsButton_Click(object sender, EventArgs e)
    {
        navigationService.NavigateTo<SettingsSidePanelView>();
    }

    private void discoveryButton_Click(object sender, EventArgs e)
    {
        UnselectAllChatCards();
        navigationService.NavigateTo<DiscoveryPanelView>();
    }

    private void flowLayoutPanelRooms_Resize(object sender, EventArgs e)
    {
        foreach (RecentRoomCard card in flowLayoutPanelRooms.Controls.OfType<RecentRoomCard>())
        {
            card.Width = flowLayoutPanelRooms.Width - flowLayoutPanelRooms.Margin.Horizontal * 2;
        }
    }

    private void searchButton_Click(object sender, EventArgs e)
    {
        var lowerQuery = roomSearchTextBox.Text.ToLowerInvariant();

        flowLayoutPanelRooms.Controls.Clear();

        foreach (var card in activeRecentRoomCards.Values
            .Where(x => x.RoomName
                .Contains(lowerQuery, StringComparison.InvariantCultureIgnoreCase)))
        {
            flowLayoutPanelRooms.Controls.Add(card);
        }
    }
}
