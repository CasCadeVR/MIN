using MIN.Core.Entities.Contracts.Models;
using MIN.Desktop.ViewModels.Pages;

namespace MIN.Desktop.Contracts.Models.ReferenceCommands;

/// <summary>
/// Команда регистриации комнаты
/// </summary>
public class RegisterRoomReferenceCommand
{
    /// <summary>
    /// RoomInfo, которую должны зарегистрировать
    /// </summary>
    public required RoomInfo Room { get; init; }

    /// <summary>
    /// Связанная с roomInfo view
    /// </summary>
    public required ChatViewModel View { get; init; }
}
