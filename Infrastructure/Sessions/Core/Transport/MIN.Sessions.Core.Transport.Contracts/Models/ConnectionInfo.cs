namespace MIN.Sessions.Core.Transport.Contracts.Models;

/// <summary>
/// Информации о подсоединении
/// </summary>
public sealed record ConnectionInfo
{
    /// <summary>
    /// Тип транспорта: "pipe" | "tcp"
    /// </summary>
    public required string Type { get; init; }

    /// <summary>
    /// Значение: pipe name или "host:port"
    /// </summary>
    public required string Value { get; init; }
}
