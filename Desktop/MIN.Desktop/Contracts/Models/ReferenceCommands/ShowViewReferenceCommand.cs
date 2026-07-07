using MIN.Desktop.ViewModels.Base.Interfaces;

namespace MIN.Desktop.Contracts.Models.ReferenceCommands;

/// <summary>
/// Команда показа View
/// </summary>
public record ShowViewReferenceCommand(IRoutableViewModel ViewModel);
