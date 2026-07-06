using MIN.Desktop.Contracts.Enums;

namespace MIN.Desktop.ViewModels.Base.Interfaces;

/// <summary>
/// ViewModel, способная навигировать
/// </summary>
public interface IRoutableViewModel
{
    /// <inheritdoc cref="ViewLayoutType"/>
    ViewLayoutType LayoutType { get; }

    /// <summary>
    /// Свящана ли эта панель с центральной, и должна ли она убраться во время перехода с неё
    /// </summary>
    bool RelatedToCentral { get; }

    /// <summary>
    /// Обновляет текущий контейнер представления, чтобы отобразить другое представление, как это определено типом TViewModel.
    /// </summary>
    void ChangeView<TViewModel>(TViewModel viewModel) where TViewModel : IRoutableViewModel;


    /// <summary>
    /// Закрывает View, используя свой layout как параметр
    /// </summary>
    void CloseView();

    /// <summary>
    /// Перейти назад
    /// </summary>
    protected void ChangeViewToPrevious();

    /// <summary>
    /// Перейти назад
    /// </summary>
    /// <typeparam name="T">Тип view model, на который нужно переместиться назад</typeparam>
    protected void ChangeViewToPrevious<T>() where T : IRoutableViewModel;
}
