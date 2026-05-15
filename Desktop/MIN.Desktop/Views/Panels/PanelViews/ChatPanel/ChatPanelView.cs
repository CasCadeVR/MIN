using MIN.Core.Entities;
using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Messaging.RoomRelated;
using MIN.Core.Transport.Contracts.Interfaces;
using MIN.Desktop.Contracts.Interfaces;
using MIN.Desktop.Contracts.Views.PanelViews;
using MIN.Desktop.Contracts.Views.PanelViews.Interfaces;
using MIN.Desktop.Infrastructure.Events;
using MIN.DI.FeatureCollection;
using MIN.Helpers.Contracts.Extensions;

namespace MIN.Desktop.Views.Panels.PanelViews.ChatPanel;

/// <summary>
/// Панель чата
/// </summary>
public partial class ChatPanelView : StyledPanelView, IPanelInitializeDepended<(Room room, Guid connectionId, IEndpoint endpoint)>, IAsyncDisposable
{
    private readonly IMinFeatureCollection featureCollection;
    private readonly INavigationService navigationService;
    private readonly CancellationTokenSource formCts = new();

    private readonly ParticipantInfo localParticipant;
    private Guid roomId;
    private Guid connectionId;
    private Room room = null!;
    private IEndpoint endpoint = null!;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="ChatPanelView"/>
    /// </summary>
    public ChatPanelView(IMinFeatureCollection featureCollection,
        INavigationService navigationService)
    {
        InitializeComponent();

        this.featureCollection = featureCollection;
        this.navigationService = navigationService;
        localParticipant = featureCollection.Helper.IdentityService.SelfParticipant.ToParticipantInfo();

        SendSystemMessage(new SystemTextMessage
        {
            Content = "Загрузка...",
        });

        InitializeNotifications();
        InitializeResizeTimer();
        InitializeTypingTimer();
        InitializeParentFormWindowStateEvents();
        HideMultiFileAttachmentUploader();
        HideStatusRow();
    }

    /// <inheritdoc />
    public override void OnNavigatedTo()
    {
        if (loadedPage == 1)
        {
            return;
        }

        loadedPage = 1;
        var lastHistory = featureCollection.Core.RoomFactory
            .GetOrCreateContext(roomId).Messages.GetRecentHistory();
        RenderMessages(lastHistory.ToList());
        ShowLoadMoreLabel();
    }

    /// <inheritdoc />
    public override void OnNavigatedFrom()
    {
        if (loadedPage > 1)
        {
            chatFlow.Controls.Clear();
        }
    }

    /// <inheritdoc />
    /// <remarks>
    /// Room передаётся по ссылке прямо из store, так что его обновление повлияет на room ui,
    /// но придётся ещё и обновить данные
    /// </remarks>
    public void Initialize((Room room, Guid connectionId, IEndpoint endpoint) parameters)
    {
        room = parameters.room;
        lastRoomName = room.Name;
        connectionId = parameters.connectionId;
        roomId = room.Id;
        endpoint = parameters.endpoint;

        SubscribeToEvents(featureCollection.Core.EventBus);
        UpdateStats();
        UpdateChatFlow();
    }

    private async Task CleanUpAsync(Guid roomId, Guid connectionId, bool isHost)
    {
        if (isHost)
        {
            await featureCollection.Core.RoomHoster.StopHostingAsync(roomId);
            await featureCollection.Discovery.DiscoveryService.StopDiscoveryAsync(roomId);
        }
        else
        {
            await featureCollection.Core.RoomConnector.DisconnectAsync(roomId, connectionId);
        }

        await featureCollection.Core.EventBus.PublishAsync(new RoomClosedEvent() { RoomId = roomId });
        featureCollection.Core.RoomFactory.DestroyContext(roomId);
        featureCollection.Core.RoomStore.Remove(roomId);
    }

    /// <inheritdoc cref="IAsyncDisposable.DisposeAsync"/>
    public async ValueTask DisposeAsync()
    {
        ClearParentFormEvents();
        foreach (var token in eventTokens)
        {
            token.Dispose();
        }
        resizeTimer.Dispose();
        typingTimer.Dispose();
        formCts.Cancel();
        formCts.Dispose();
        await CleanUpAsync(roomId, connectionId, isHost: localParticipant.Id == room.HostParticipant.Id);
    }
}
