using MIN.Core.Entities.Contracts.Models;
using MIN.Desktop.ViewModels.Pages.ChatViewModels;

namespace MIN.Desktop.Contracts.Models.ReferenceCommands;

/// <summary>
/// Команда регистриации комнаты
/// </summary>
public record RegisterRoomReferenceCommand(RoomInfo Room, ChatViewModel View);
