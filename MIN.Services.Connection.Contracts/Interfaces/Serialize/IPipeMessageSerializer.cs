namespace MIN.Services.Connection.Contracts.Interfaces.Serialize
{
    /// <summary>
    /// Сериализатор сообщений
    /// </summary>
    public interface IPipeMessageSerializer
    {
        /// <summary>
        /// Читает сообщение из pipe
        /// Формат сообщения: [4 байта: длина][1 байт: тип][N байт: данные]
        /// Типы: 0 = ChatMessage, 1 = RoomInfoMessage
        /// </summary>
        Task<object> ReadMessageAsync(Stream stream, CancellationToken cancellationToken);

        /// <summary>
        /// Пишет сообщение в pipe
        /// Типы: 0 = ChatMessage, 1 = RoomInfoMessage
        /// </summary>
        Task WriteMessageAsync<T>(Stream stream, T message, CancellationToken ct = default) where T : class;
    }
}
