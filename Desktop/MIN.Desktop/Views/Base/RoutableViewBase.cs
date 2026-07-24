using Avalonia.Controls;
using MIN.Desktop.ViewModels.Base.Interfaces;

namespace MIN.Desktop.Views.Base;

/// <summary>
/// Базовая view для навигационных view
/// </summary>
public class RoutableViewBase<TViewModel> : UserControl
    where TViewModel : IRoutableViewModel;
