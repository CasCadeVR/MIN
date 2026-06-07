using MIN.Sessions.Core.Transport.Contracts.Enums;

namespace MIN.Sessions.Core.Transport.Contracts.Models;

/// <summary>
/// Соединение с приложением
/// </summary>
public record TransportConnection(
    Guid RoomId,
    SessionProcessRole Role,
    int SubRoomId,
    Stream InputStream,
    Stream OutputStream);
