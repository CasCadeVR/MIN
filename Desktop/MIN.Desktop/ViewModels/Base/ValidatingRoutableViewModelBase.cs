using System;
using CommunityToolkit.Mvvm.Messaging;
using MIN.Desktop.Contracts.Enums;
using MIN.Desktop.Contracts.Models.ReferenceCommands.Layout;
using MIN.Desktop.ViewModels.Base.Interfaces;

namespace MIN.Desktop.ViewModels.Base;

/// <summary>
/// Валидирующая view модель для страниц
/// </summary>
public abstract class ValidatingRoutableViewModelBase : ValidatingViewModelBase, IRoutableViewModel, IViewModel
{
    /// <inheritdoc cref="ViewLayoutType"/>
    public abstract ViewLayoutType LayoutType { get; }

    /// <inheritdoc />
    public virtual bool RelatedToCentral { get; }

    /// <inheritdoc />
    public virtual EventHandler? OnNavigatedTo { get; }

    /// <inheritdoc />
    public virtual EventHandler? OnNavigatedFrom { get; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="ValidatingRoutableViewModelBase"/>
    /// </summary>
    protected ValidatingRoutableViewModelBase() : base() { }

    /// <inheritdoc />
    public void ChangeView<TViewModel>(TViewModel viewModel) where TViewModel : IRoutableViewModel
    {
        WeakReferenceMessenger.Default.Send(new ShowViewReferenceCommand(viewModel));
        viewModel.OnNavigatedTo?.Invoke(this, EventArgs.Empty);
    }

    /// <inheritdoc />
    public void CloseView(object? sender = null)
    {
        OnNavigatedFrom?.Invoke(sender ?? this, EventArgs.Empty);
        WeakReferenceMessenger.Default.Send(new CloseViewReferenceCommand(LayoutType));
    }

    /// <inheritdoc />
    public void ChangeViewToPrevious(object? sender = null)
    {
        OnNavigatedFrom?.Invoke(sender ?? this, EventArgs.Empty);
        WeakReferenceMessenger.Default.Send(new ShowPreviousViewReferenceCommand(LayoutType, null));
    }

    /// <inheritdoc />
    public void ChangeViewToPrevious<T>(object? sender = null) where T : IRoutableViewModel
    {
        OnNavigatedFrom?.Invoke(sender ?? this, EventArgs.Empty);
        WeakReferenceMessenger.Default.Send(new ShowPreviousViewReferenceCommand(LayoutType, typeof(T)));
    }
}
