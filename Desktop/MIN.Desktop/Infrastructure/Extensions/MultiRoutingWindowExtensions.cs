using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MIN.Common.Core.Extensions;
using MIN.Desktop.Contracts.Enums;
using MIN.Desktop.Contracts.Interfaces;
using MIN.Desktop.ViewModels.Base;
using MIN.Desktop.ViewModels.Base.Interfaces;

namespace MIN.Desktop.Infrastructure.Extensions;

/// <summary>
/// Расширения для <see cref="IMultiRoutingWindow"/>
/// </summary>
public static class MultiRoutingWindowExtensions
{
    private readonly static Dictionary<ViewLayoutType, List<IRoutableViewModel>> navigationStack = new()
    {
        { ViewLayoutType.LeftSideBar, [] },
        { ViewLayoutType.Central, [] },
        { ViewLayoutType.RightSideBar, [] },
    };
    //private static CancellationTokenSource? viewChangeBusyCts;

    /// <summary>
    /// Переходит к представлению, назначенному данной ViewModel
    /// </summary>
    /// <param name="screen">Экран, используемый для отображения изображения.</param>
    /// <param name="routableViewModel">ViewModel, которую следует отобразить</param>
    /// <typeparam name="TViewModel">Тип модели представления для отображения</typeparam>
    public static async Task ShowAsync<TViewModel>(this IMultiRoutingWindow? screen, TViewModel routableViewModel) where TViewModel : IRoutableViewModel
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

        if (routableViewModel.LayoutType == ViewLayoutType.LeftSideBar
            && screen.LeftSideBarViewModel is IRoutableViewModel leftPriorViewModel)
        {
            navigationStack[routableViewModel.LayoutType].RemoveByPredicate(screen.LeftSideBarViewModel, (item, param) => item.GetType() == param.GetType());
            navigationStack[routableViewModel.LayoutType].Add(leftPriorViewModel);
        }
        else if (routableViewModel.LayoutType == ViewLayoutType.Central
            && screen.CentralViewModel is IRoutableViewModel centralPriorViewModel)
        {
            navigationStack[routableViewModel.LayoutType].RemoveByPredicate(screen.CentralViewModel, (item, param) => item.GetType() == param.GetType());
            navigationStack[routableViewModel.LayoutType].Add(centralPriorViewModel);

            if (screen.RightSideBarViewModel is IRoutableViewModel rightPriorViewModel
                && rightPriorViewModel.RelatedToCentral)
            {
                if (screen.RightSideBarViewModel is RoutableViewModelBase routableRightSidebar)
                {
                    routableRightSidebar.CloseView(routableViewModel);
                }
                else
                {
                    screen.RightSideBarViewModel = null;
                }
            }
        }
        else if (routableViewModel.LayoutType == ViewLayoutType.RightSideBar
            && screen.RightSideBarViewModel is IRoutableViewModel rightPriorViewModel)
        {
            navigationStack[routableViewModel.LayoutType].RemoveByPredicate(screen.RightSideBarViewModel, (item, param) => item.GetType() == param.GetType());
            navigationStack[routableViewModel.LayoutType].Add(rightPriorViewModel);
        }
        else
        {
            leftPriorViewModel = null!;
            centralPriorViewModel = null!;
            rightPriorViewModel = null!;
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
                    screen.CentralViewModel = routableViewModel;
                    break;

                case ViewLayoutType.RightSideBar:
                    screen.RightSideBarViewModel = routableViewModel;
                    break;

                default:
                    screen.CentralViewModel = routableViewModel;
                    break;
            }
        }
        catch (OperationCanceledException)
        {
            if (navigationStack.Count > 0)
            {
                //if (leftPriorViewModel != null)
                //{
                //    navigationStack[leftPriorViewModel.LayoutType].Remove(navigationStack[leftPriorViewModel.LayoutType][^1]);
                //}
                //if (centralPriorViewModel != null)
                //{
                //    navigationStack[centralPriorViewModel.LayoutType].Remove(navigationStack[centralPriorViewModel.LayoutType][^1]);
                //}
                //if (rightPriorViewModel != null)
                //{
                //    navigationStack[rightPriorViewModel.LayoutType].Remove(navigationStack[rightPriorViewModel.LayoutType][^1]);
                //}
            }
        }
    }

    /// <summary>
    /// Показать предыдущий экран
    /// </summary>
    public static async Task<bool> BackAsync(this IMultiRoutingWindow screen, ViewLayoutType viewLayoutType)
    {
        IRoutableViewModel? backViewModel = null;
        while (navigationStack.Count > 0 && (backViewModel == null
            || backViewModel == screen.LeftSideBarViewModel
            || backViewModel == screen.CentralViewModel
            || backViewModel == screen.RightSideBarViewModel))
        {
            backViewModel = navigationStack[viewLayoutType][^1];
            navigationStack[backViewModel.LayoutType].Remove(backViewModel);
        }
        if (backViewModel != null)
        {
            await ShowAsync(screen, backViewModel);
        }
        return true;
    }

    /// <summary>
    /// Пытается вернуться к представлению, назначенному данной ViewModel
    /// </summary>
    /// <returns>
    /// Значение True, если ViewModel был найден в стеке навигации маршрутизации. Значение False, если ViewModel не был найден и маршрутизация
    /// завершилась неудачей.
    /// </returns>
    public static async Task<bool> BackToAsync(this IMultiRoutingWindow screen, Type? type, ViewLayoutType layoutType)
    {
        if (type == null)
        {
            return await BackAsync(screen, layoutType);
        }

        for (var i = navigationStack.Count - 1; i >= 0; i--)
        {
            var target = navigationStack[layoutType][i];
            if (type.IsAssignableFrom(target.GetType()))
            {
                // Cleanup the stack up and including the back-target.
                for (var j = i; j < navigationStack.Count; j++)
                {
                    navigationStack[target.LayoutType].RemoveAt(j);
                }
                await screen.ShowAsync(target);
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Закрыть окно
    /// </summary>
    public static async Task CloseAsync(this IMultiRoutingWindow screen, ViewLayoutType viewLayoutType)
    {
        switch (viewLayoutType)
        {
            case ViewLayoutType.LeftSideBar:
                screen.LeftSideBarViewModel = null;
                break;

            case ViewLayoutType.Central:
                screen.CentralViewModel = null;
                ResetRelated(screen);
                break;

            case ViewLayoutType.RightSideBar:
                screen.RightSideBarViewModel = null;
                break;
        }
    }

    private static void ResetRelated(IMultiRoutingWindow screen)
    {
        if (screen.RightSideBarViewModel is IRoutableViewModel rightPriorViewModel
            && rightPriorViewModel.RelatedToCentral)
        {
            screen.RightSideBarViewModel = null;
        }
    }
}
