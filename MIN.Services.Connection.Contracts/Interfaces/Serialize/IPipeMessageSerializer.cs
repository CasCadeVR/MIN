namespace MIN.Services.Connection.Contracts.Interfaces.Serialize
{
    /// <summary>
    /// Сериализатор сообщений
    /// </summary>
    public interface IPipeMessageSerializer
    {
        /// <summary>
        /// Имя комнаты, для которой ведётся передача сообщений
        /// </summary>
        string RoomName { get; set; }

        /// <summary>
        /// Читает сообщение из pipe
        /// Формат сообщения: [4 байта: длина][1 байт: тип][N байт: данные]
        /// </summary>
        Task<object> ReadMessageAsync(Stream stream, CancellationToken cancellationToken);

        /// <summary>
        /// Пишет сообщение в pipe
        /// </summary>
        Task WriteMessageAsync<T>(Stream stream, T message, CancellationToken cancellationToken = default) where T : class;
    }
}
