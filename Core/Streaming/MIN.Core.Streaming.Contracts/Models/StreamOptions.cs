namespace MIN.Core.Streaming.Contracts.Models;

/// <summary>
/// Настройки потока
/// </summary>
public sealed class StreamOptions
{
    /// <summary>
    /// Нужно ли подтверждать получение пакетов со стороны получателя
    /// </summary>
    public bool RequiresAcks { get; init; }

    /// <summary>
    /// Нужно ли шифровать чанки
    /// </summary>
    public bool RequiresEncryption { get; init; }

    /// <summary>
    /// Пользовательский идентификатор потока. Если не задан, генерируется автоматически.
    /// Используется для связи потока с бизнес-логикой (например, TransferId).
    /// </summary>
    public Guid? StreamId { get; init; }

    /// <summary>
    /// Указывает, что поток содержит сырые байтовые данные (например, файл),
    /// которые не должны десериализоваться как JSON-сообщение.
    /// </summary>
    public bool IsRawPayload { get; init; }
}
