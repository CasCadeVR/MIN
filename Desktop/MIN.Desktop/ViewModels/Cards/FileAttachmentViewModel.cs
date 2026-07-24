using System;
using CommunityToolkit.Mvvm.Input;
using MIN.Desktop.Contracts.Models;
using MIN.Desktop.ViewModels.Base;

namespace MIN.Desktop.ViewModels.Cards;

/// <summary>
/// View модель приложенного файла
/// </summary>
public partial class FileAttachmentViewModel : CardViewModelBase, IDisposable
{
    /// <summary>
    /// Приложенный файл
    /// </summary>
    public FileAttachment File { get; }

    /// <summary>
    /// Событие по удалению файла
    /// </summary>
    public Action? OnDelete { get; set; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="FileAttachmentViewModel"/>
    /// </summary>
    public FileAttachmentViewModel(FileAttachment file)
    {
        File = file;
    }

    /// <summary>
    /// Удалить файл
    /// </summary>
    [RelayCommand]
    public void OnDeleteClick()
    {
        OnDelete?.Invoke();
    }
}
