namespace MIN.Services.Connection.Contracts.Models.Enums
{
    /// <summary>
    /// Тэг сообщения
    /// </summary>
    public enum MessageTypeTag : byte
    {
        HandshakeMessage = 0,
        DiscoveredRoom = 1,
        RoomInfo = 2,
        ChatMessage = 3,
    }
}
