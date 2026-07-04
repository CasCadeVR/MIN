using System;
using MIN.Desktop.Contracts.Enums;

namespace MIN.Desktop.Contracts.Models.ReferenceCommands;

/// <summary>
/// Команда показа предыдущего View
/// </summary>
public class ShowPreviousViewReferenceCommand
{
    /// <summary>
    /// Тип страницы, куда нужно показать view
    /// </summary>
    public ViewLayoutType LayoutType { get; init; }

    /// <summary>
    /// ViewModel, которую должны показать
    /// </summary>
    public Type? RoutableViewModelType { get; init; } = null!;
}
