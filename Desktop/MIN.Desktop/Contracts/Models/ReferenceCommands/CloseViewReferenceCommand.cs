using MIN.Desktop.Contracts.Enums;

namespace MIN.Desktop.Contracts.Models.ReferenceCommands;

/// <summary>
/// Команда закрытия View
/// </summary>
public record CloseViewReferenceCommand(ViewLayoutType LayoutType);
