using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Events.Events;
using MIN.Desktop.Components;
using MIN.Desktop.Contracts.Enums;
using MIN.Desktop.Contracts.Interfaces;
using MIN.Desktop.Contracts.Schemes;
using MIN.Desktop.Contracts.Views.PanelViews;
using MIN.Desktop.Infrastructure.Services;
using MIN.Desktop.Views.Panels.PanelViews.ChatPanel;
using MIN.DI.FeatureCollection;
using MIN.Helpers.Contracts.Extensions;

namespace MIN.Desktop.Views.Panels.SidePanelViews;

/// <summary>
/// Главная боковая панель
/// </summary>
public partial class MainSidePanelView : StyledPanelView, IChatPanelManager
{
    private readonly IMinFeatureCollection featureCollection;
    private readonly INavigationService navigationService;
    private readonly Dictionary<Guid, RecentRoomCard> activeRecentRoomCards = [];
    private readonly Dictionary<Guid, ChatPanelView> activeChatPanels = [];
    private readonly ParticipantInfo localParticipant;
    private RecentRoomCard? selectedRecentRoomCard;

    /// <inheritdoc />
    public override PanelType PanelType => PanelType.Side;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="MainSidePanelView"/>
    /// </summary>
    public MainSidePanelView(IMinFeatureCollection featureCollection,
        INavigationService navigationService)
    {
        InitializeComponent();

        this.featureCollection = featureCollection;
        this.navigationService = navigationService;

        localParticipant = featureCollection.Helper.IdentityService.SelfParticipant.ToParticipantInfo();

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

    void IChatPanelManager.RegisterChat(RoomInfo roomInfo, ChatPanelView panel)
    {
        var roomId = roomInfo.Id;
        var context = featureCollection.Core.RoomFactory.GetOrCreateContext(roomId);

        activeChatPanels[roomId] = panel;
        var card = new RecentRoomCard(featureCollection.Core.EventBus,
            context,
            roomInfo,
            AsCreator: roomInfo.HostParticipant.Id == localParticipant.Id)
        {
            Width = flowLayoutPanelRooms.Width - flowLayoutPanelRooms.Margin.Horizontal * 2,
        };

        card.Clicked += () =>
        {
            if (selectedRecentRoomCard != card)
            {
                SelectChatCard(card);
                navigationService.NavigateToExisting(panel);
            }
        };
        flowLayoutPanelRooms.Controls.Add(card);
        activeRecentRoomCards[roomId] = card;
        SelectChatCard(card);
    }

    private void UnselectRecentRoomCard()
    {
        if (selectedRecentRoomCard != null)
        {
            selectedRecentRoomCard.IsSelected = false;
            selectedRecentRoomCard.BackColor = ColorScheme.Transparent;
        }

        selectedRecentRoomCard = null;
    }

    private void SelectChatCard(RecentRoomCard card)
    {
        UnselectRecentRoomCard();
        selectedRecentRoomCard = card;
        card.BackColor = ColorScheme.SecondaryAccent;
        card.SelectCard();
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
        UnselectRecentRoomCard();
        navigationService.NavigateTo<DiscoveryPanelView>();
    }

    private void flowLayoutPanelRooms_Resize(object sender, EventArgs e)
    {
        foreach (RecentRoomCard card in flowLayoutPanelRooms.Controls.OfType<RecentRoomCard>())
        {
            card.Width = flowLayoutPanelRooms.Width - flowLayoutPanelRooms.Margin.Horizontal * 2;
        }
    }

    private void PerformRecentRoomSearch()
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

    private void searchButton_Click(object sender, EventArgs e)
    {
        PerformRecentRoomSearch();
    }

    private void roomSearchTextBox_TextChanged(object sender, EventArgs e)
    {
        PerformRecentRoomSearch();
    }
}
