
using MIN.Core.Events.Contracts;

namespace MIN.Discovery.Events;

/// <summary>
/// Событие прошедшей проверки конечного компьютера
/// </summary>
public class EndpointCheckedEvent : BaseEvent
{
    /// <summary>
    /// Конечный компьютер
    /// </summary>
    public string Endpoint { get; init; } = string.Empty;

    /// <summary>
    /// Успешно ли прошла проверка
    /// </summary>
    public bool IsSuccessful { get; init; }
}
