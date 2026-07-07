using System;
using MIN.Desktop.Contracts.Enums;

namespace MIN.Desktop.Contracts.Models.ReferenceCommands;

/// <summary>
/// Команда показа предыдущего View
/// </summary>
public record ShowPreviousViewReferenceCommand(ViewLayoutType LayoutType, Type? RoutableViewModelType);
