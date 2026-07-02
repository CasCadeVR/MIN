using MIN.Desktop.ViewModels.Base;

namespace MIN.Desktop.Contracts.Models.ReferenceCommands;

/// <summary>
/// Команда показа View
/// </summary>
public class ShowViewReferenceCommand
{
    /// <summary>
    /// ViewModel, которую должны показать
    /// </summary>
    public required RoutableViewModelBase ViewModel { get; init; }
}
