using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MIN.Common.Core.Extensions;
using MIN.Desktop.Contracts.Enums;
using MIN.Desktop.Contracts.Interfaces;
using MIN.Desktop.ViewModels.Base;

namespace MIN.Desktop.Infrastructure.Extensions;

/// <summary>
/// Расширения для <see cref="IMultiRoutingWindow"/>
/// </summary>
public static class MultiRoutingWindowExtensions
{
    private readonly static Dictionary<ViewLayoutType, List<RoutableViewModelBase>> navigationStack = [];
    //private static CancellationTokenSource? viewChangeBusyCts;

    /// <summary>
    ///     Navigates to a view assigned to the given ViewModel.
    /// </summary>
    /// <param name="screen">The screen used to display the view.</param>
    /// <param name="routableViewModel">ViewModel that should be shown.</param>
    /// <typeparam name="TViewModel">Type of the ViewModel to show.</typeparam>
    public static async Task ShowAsync<TViewModel>(this IMultiRoutingWindow? screen, TViewModel routableViewModel) where TViewModel : RoutableViewModelBase
    {
        if (screen == null)
        {
            return;
        }
        //CancellationToken ctsToken;
        //if (viewChangeBusyCts != null)
        //{
        //    viewChangeBusyCts.Cancel();
        //    viewChangeBusyCts.Dispose();
        //}
        //viewChangeBusyCts = new CancellationTokenSource();
        //ctsToken = viewChangeBusyCts.Token;

        if (screen.ActiveViewModel is RoutableViewModelBase priorViewModel)
        {
            navigationStack[priorViewModel.LayoutType].RemoveByPredicate(screen.ActiveViewModel, (item, param) => item.GetType() == param.GetType());
            navigationStack[priorViewModel.LayoutType].Add(priorViewModel);
        }
        else
        {
            priorViewModel = null!;
        }

        try
        {
            //    ctsToken.ThrowIfCancellationRequested();
            //    var sw = Stopwatch.StartNew();
            //    Task contentLoadTask = routableViewModel.ViewContentLoadAsync(ctsToken);
            //    if (screen.ActiveViewModel != null)
            //    {
            //        // Only show loading screen if page isn't loading super quickly.
            //        await Task.Delay(50, ctsToken);
            //        if (!contentLoadTask.IsCompleted)
            //        {
            //            ctsToken.ThrowIfCancellationRequested();
            //            screen.ActiveViewModel = AssetHelper.GetFullAssetPath("/Assets/Icons/loading.svg");
            //            await Task.Delay((int)Math.Max(0, 500 - sw.Elapsed.TotalMilliseconds), ctsToken);
            //        }
            //    }
            //    await contentLoadTask;
            //    ctsToken.ThrowIfCancellationRequested();
            switch (routableViewModel.LayoutType)
            {
                case ViewLayoutType.LeftSideBar:
                    screen.LeftSideBarViewModel = routableViewModel;
                    break;

                case ViewLayoutType.Central:
                    screen.ActiveViewModel = routableViewModel;
                    break;

                case ViewLayoutType.RightSideBar:
                    screen.RightSideBarViewModel = routableViewModel;
                    break;

                default:
                    screen.ActiveViewModel = routableViewModel;
                    break;
            }
        }
        catch (OperationCanceledException)
        {
            if (priorViewModel != null && navigationStack.Count > 0)
            {
                navigationStack[priorViewModel.LayoutType].Remove(navigationStack[priorViewModel.LayoutType][^1]);
            }
        }
    }

    /// <summary>
    /// Показать предыдущий экран
    /// </summary>
    public static async Task<bool> BackAsync(this IMultiRoutingWindow screen)
    {
        RoutableViewModelBase? backViewModel = null;
        while (navigationStack.Count > 0 && (backViewModel == null || backViewModel == screen.ActiveViewModel))
        {
            backViewModel = navigationStack[backViewModel?.LayoutType ?? ViewLayoutType.Central][^1];
            navigationStack[backViewModel.LayoutType].Remove(backViewModel);
        }
        if (backViewModel != null)
        {
            await ShowAsync(screen, backViewModel);
        }
        return true;
    }

    /// <summary>
    ///     Tries to go back to the view assigned to the given ViewModel.
    /// </summary>
    /// <returns>
    ///     True if ViewModel was found in the routing navigation stack. False when the ViewModel wasn't found and routing
    ///     failed.
    /// </returns>
    public static async Task<bool> BackToAsync(this IMultiRoutingWindow screen, Type? type, ViewLayoutType layoutType)
    {
        if (type == null)
        {
            return await BackAsync(screen);
        }

        for (var i = navigationStack.Count - 1; i >= 0; i--)
        {
            RoutableViewModelBase target = navigationStack[layoutType][i];
            if (type.IsAssignableFrom(target.GetType()))
            {
                // Cleanup the stack up and including the back-target.
                for (int j = i; j < navigationStack.Count; j++)
                {
                    navigationStack[target.LayoutType].RemoveAt(j);
                }
                await screen.ShowAsync(target);
                return true;
            }
        }
        return false;
    }
}
