using MIN.Desktop.Contracts.Enums;

namespace MIN.Desktop.Contracts.Models.ReferenceCommands;

/// <summary>
/// Команда закрытия View
/// </summary>
public class CloseViewReferenceCommand
{
    /// <summary>
    /// Тип страницы, которое нужно закрыть
    /// </summary>
    public ViewLayoutType LayoutType { get; init; }
}
