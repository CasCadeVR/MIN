using Avalonia.Controls;
using MIN.Desktop.ViewModels.Base.Interfaces;

namespace MIN.Desktop.Views.Base;

/// <summary>
/// Базовая view для модальных окон
/// </summary>
public class RoutableViewBase<TViewModel> : UserControl
    where TViewModel : IRoutableViewModel;
