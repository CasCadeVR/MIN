using MIN.Desktop.ViewModels.Base.Interfaces;

namespace MIN.Desktop.Contracts.Models.ReferenceCommands.Layout;

/// <summary>
/// Команда показа View
/// </summary>
public record ShowViewReferenceCommand(IRoutableViewModel ViewModel);
