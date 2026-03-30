using MIN.Core.Messaging.Contracts.Interfaces;

namespace MIN.Handlers.Contracts.Models;

/// <summary>
/// Результат обработки сообщения
/// </summary>
public sealed class HandlerResult
{
    /// <summary>
    /// Успешность обработки
    /// </summary>
    public bool IsSuccess { get; init; }

    /// <summary>
    /// Флаг, указывающий, следует ли остановить дальнейшую обработку (для цепочек)
    /// </summary>
    public bool StopPropagation { get; init; }

    /// <summary>
    /// Ответное сообщение (если требуется)
    /// </summary>
    public IMessage? Response { get; init; }

    /// <summary>
    /// Сообщение об ошибке (если не успешно)
    /// </summary>
    public string? ErrorMessage { get; init; }

    /// <summary>
    /// Создаёт успешный результат
    /// </summary>
    public static HandlerResult Success(bool stopPropagation = false)
        => new()
        {
            IsSuccess = true,
            StopPropagation = stopPropagation
        };

    /// <summary>
    /// Создаёт результат с ошибкой
    /// </summary>
    public static HandlerResult Failure(string errorMessage, bool stopPropagation = true)
        => new()
        {
            IsSuccess = false,
            StopPropagation = stopPropagation,
            ErrorMessage = errorMessage
        };

    /// <summary>
    /// Создаёт результат с ответным сообщением
    /// </summary>
    public static HandlerResult WithResponse(IMessage response, bool stopPropagation = false)
        => new()
        {
            IsSuccess = true,
            StopPropagation = stopPropagation,
            Response = response
        };
}
