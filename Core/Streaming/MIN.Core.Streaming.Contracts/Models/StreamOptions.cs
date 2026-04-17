using MIN.Core.Streaming.Contracts.Constants;
using MIN.Core.Transport.Contracts.Constants;

namespace MIN.Core.Streaming.Contracts.Models;

/// <summary>
/// Настройки потока
/// </summary>
public sealed class StreamOptions
{
    /// <summary>
    /// Идентфикатор потока
    /// </summary>
    public Guid StreamId { get; init; } = Guid.NewGuid();

    /// <summary>
    /// Нужно ли подтверждать получение пакетов со стороны получателя
    /// </summary>
    public bool RequiresAcks { get; init; }

    /// <summary>
    /// Нужно ли шифровать чанки
    /// </summary>
    public bool RequiresEncryption { get; init; }
}
