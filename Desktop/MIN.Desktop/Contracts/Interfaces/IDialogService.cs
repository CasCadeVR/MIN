using System;
using System.Threading.Tasks;
using MIN.Desktop.ViewModels.Base;

namespace MIN.Desktop.Contracts.Interfaces;

/// <summary>
/// Сервис для предоставления диалоговых окон
/// </summary>
public interface IDialogService
{
    /// <summary>
    /// Показать модальное окно
    /// </summary>
    Task<TViewModel?> ShowAsync<TViewModel>(Action<TViewModel>? viewModelSetup = null)
        where TViewModel : ModalViewModelBase;

    /// <summary>
    /// Показать ошибку в виде модального окна
    /// </summary>
    Task ShowErrorAsync(Exception exception, string? title = null, string? description = null);
}
