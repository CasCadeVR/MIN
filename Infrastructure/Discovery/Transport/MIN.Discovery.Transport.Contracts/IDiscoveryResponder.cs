namespace MIN.Discovery.Transport.Contracts;

/// <summary>
/// Ответчик на запрос обнаружения
/// </summary>
public interface IDiscoveryResponder
{
    /// <summary>
    /// Ответить на запрос
    /// </summary>
    Task RespondAsync(byte[] data, CancellationToken ct = default);
}
