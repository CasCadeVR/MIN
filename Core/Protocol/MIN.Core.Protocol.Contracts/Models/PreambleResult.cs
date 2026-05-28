using MIN.Core.Entities.Contracts.Models;

namespace MIN.Core.Protocol.Contracts.Models;

/// <summary>
/// Результат общения обменом протокола
/// </summary>
public sealed class PreambleResult
{
    /// <summary>
    /// Успешно ли прошло общение
    /// </summary>
    public bool IsSuccess { get; init; }

    /// <summary>
    /// Сообщение об ошибке (если неуспешно)
    /// </summary>
    public string? ErrorMessage { get; init; }

    /// <summary>
    /// Полученная информация о комнате
    /// </summary>
    /// <remarks>
    /// Нужен только для подключаемого
    /// </remarks>
    public RoomInfo RoomInfo { get; init; } = null!;
}
