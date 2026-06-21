using MIN.Sessions.Core.Transport.Contracts.Enums;

namespace MIN.Sessions.Core.Transport.Contracts.Models;

/// <summary>
/// Контекст общения с приложением
/// </summary>
/// <param name="RoomId">Идентификатор комнаты</param>
/// <param name="SubRoomId">Идентификатор подкомнаты</param>
/// <param name="Role">Роль приложения</param>
public record struct ProcessContext(
    Guid RoomId,
    int SubRoomId,
    SessionProcessRole Role
);
