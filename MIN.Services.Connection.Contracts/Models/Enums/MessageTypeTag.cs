namespace MIN.Services.Connection.Contracts.Models.Enums
{
    /// <summary>
    /// Тэг сообщения
    /// </summary>
    public enum MessageTypeTag : byte
    {
        HandshakeMessage = 0,
        DiscoveredRoom = 1,
        ChatMessage = 2,
        RoomInfo = 3,
        RoomInfoRequest = 4,
    }
}
