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
        /// </summary>
        Task<object> ReadMessageAsync(Stream stream, Guid senderId, CancellationToken cancellationToken);

        /// <summary>
        /// Пишет сообщение в pipe
        /// </summary>
        Task WriteMessageAsync<T>(Stream stream, T message, Guid recipientId, CancellationToken cancellationToken = default) where T : class;
    }
}
