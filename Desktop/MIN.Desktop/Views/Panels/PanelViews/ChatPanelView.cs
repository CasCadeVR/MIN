using MIN.Desktop.Contracts.Views.PanelViews;
using MIN.Desktop.Infrastructure.Events;
using MIN.DI;

namespace MIN.Desktop.Views.Panels.PanelViews;

/// <summary>
/// Панель чата
/// </summary>
public partial class ChatPanelView : StyledPanelView
{
    private readonly IMinFeatureCollection featureCollection;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="ChatPanelView"/>
    /// </summary>
    public ChatPanelView(IMinFeatureCollection featureCollection)
    {
        InitializeComponent();
        this.featureCollection = featureCollection;
    }

    private async Task CleanUpAsync(Guid roomId, Guid connectionId, bool isHost)
    {
        if (isHost)
        {
            await featureCollection.Discovery.DiscoveryService.StopDiscoveryAsync(roomId);
            await featureCollection.Core.RoomHoster.StopHostingAsync(roomId);
        }
        else
        {
            await featureCollection.Core.RoomConnector.DisconnectAsync(roomId, connectionId);
        }

        await featureCollection.Core.EventBus.PublishAsync(new RoomClosedEvent() { RoomId = roomId });
        featureCollection.Core.RoomFactory.DestroyContext(roomId);
        featureCollection.Core.RoomStore.Remove(roomId);
    }

}
