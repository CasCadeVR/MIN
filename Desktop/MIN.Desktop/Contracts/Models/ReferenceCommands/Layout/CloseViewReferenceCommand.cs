using MIN.Desktop.Contracts.Enums;

namespace MIN.Desktop.Contracts.Models.ReferenceCommands.Layout;

/// <summary>
/// Команда закрытия View
/// </summary>
public record CloseViewReferenceCommand(ViewLayoutType LayoutType);
