using System;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;
using MIN.Desktop.Contracts.Enums;
using MIN.Desktop.Contracts.Models.ReferenceCommands.Layout;
using MIN.Desktop.ViewModels.Base.Interfaces;

namespace MIN.Desktop.ViewModels.Base;

/// <summary>
/// Базовая view модель для страниц
/// </summary>
public abstract class RoutableViewModelBase : ViewModelBase, IRoutableViewModel
{
    /// <inheritdoc cref="ViewLayoutType"/>
    public abstract ViewLayoutType LayoutType { get; }

    /// <inheritdoc />
    public virtual bool RelatedToCentral { get; }

    /// <inheritdoc />
    public virtual EventHandler? OnNavigatedTo { get; }

    /// <inheritdoc />
    public virtual EventHandler? OnNavigatedFrom { get; }

    /// <inheritdoc />
    public void ChangeView<TViewModel>(TViewModel viewModel) where TViewModel : IRoutableViewModel
    {
        WeakReferenceMessenger.Default.Send(new ShowViewReferenceCommand(viewModel));
    }

    /// <inheritdoc />
    public void CloseView(object? sender = null)
    {
        OnNavigatedFrom?.Invoke(sender, EventArgs.Empty);
        WeakReferenceMessenger.Default.Send(new CloseViewReferenceCommand(LayoutType));
    }

    /// <inheritdoc />
    public void ChangeViewToPrevious()
    {
        WeakReferenceMessenger.Default.Send(new ShowPreviousViewReferenceCommand(LayoutType, null));
    }

    /// <inheritdoc />
    public void ChangeViewToPrevious<T>() where T : IRoutableViewModel
    {
        WeakReferenceMessenger.Default.Send(new ShowPreviousViewReferenceCommand(LayoutType, typeof(T)));
    }

    /// <inheritdoc />
    public virtual Task ViewContentLoadAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    /// <inheritdoc />
    public virtual Task ViewContentUnloadAsync() => Task.CompletedTask;
}
