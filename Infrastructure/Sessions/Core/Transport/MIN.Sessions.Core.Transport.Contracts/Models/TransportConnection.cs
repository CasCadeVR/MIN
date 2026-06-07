namespace MIN.Sessions.Core.Transport.Contracts.Models;

/// <summary>
/// Соединение с приложением
/// </summary>
public record TransportConnection(
    string Role,           // "server" | "client"
    int SubRoomId,
    Stream InputStream,
    Stream OutputStream);
