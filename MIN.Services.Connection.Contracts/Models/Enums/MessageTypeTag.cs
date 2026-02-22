namespace MIN.Services.Connection.Contracts.Models.Enums
{
    /// <summary>
    /// Тэг сообщения
    /// </summary>
    public enum MessageTypeTag : byte
    {
        ChatMessage = 0,
        RoomInfo = 1,
        DiscoveredRoom = 2,
    }
}
