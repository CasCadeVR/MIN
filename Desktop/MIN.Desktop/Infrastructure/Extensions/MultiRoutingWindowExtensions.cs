using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MIN.Desktop.Contracts.Enums;
using MIN.Desktop.Contracts.Interfaces;
using MIN.Desktop.Infrastructure.Services;
using MIN.Desktop.ViewModels.Base;
using MIN.Desktop.ViewModels.Base.Interfaces;

namespace MIN.Desktop.Infrastructure.Extensions;

/// <summary>
/// Расширения для <see cref="IMultiRoutingWindow"/>
/// </summary>
public static class MultiRoutingWindowExtensions
{
    private const int LoadingAssetMinWaitMs = 10;
    private const int LoadingAssetMaximumWaitMs = 500;

    private static object? rememberedRightSideBar;
    private static object? rememberedLeftSideBar;
    private static object? rememberedCentral;

    /// <summary>
    /// Переходит к представлению, назначенному данной ViewModel
    /// </summary>
    /// <param name="screen">Экран, используемый для отображения изображения.</param>
    /// <param name="routableViewModel">ViewModel, которую следует отобразить</param>
    /// <param name="cancellationToken">Токен отмены, в случае отмены перехода</param>
    /// <typeparam name="TViewModel">Тип модели представления для отображения</typeparam>
    public static async Task ShowAsync<TViewModel>(this IMultiRoutingWindow? screen, TViewModel routableViewModel, CancellationToken cancellationToken = default) where TViewModel : IRoutableViewModel
    {
        if (screen == null)
        {
            return;
        }

        var contextViewModel = screen.GetViewModelOutOfLayoutType(routableViewModel.LayoutType);

        CancellationTokenSource routingCts;
        if (screen.ViewChangeBusyCtsByLayout[routableViewModel.LayoutType] != null)
        {
            screen.ViewChangeBusyCtsByLayout[routableViewModel.LayoutType]!.Cancel();
            screen.ViewChangeBusyCtsByLayout[routableViewModel.LayoutType]!.Dispose();
        }
        screen.ViewChangeBusyCtsByLayout[routableViewModel.LayoutType] = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        routingCts = screen.ViewChangeBusyCtsByLayout[routableViewModel.LayoutType]!;

        if (routableViewModel.LayoutType == ViewLayoutType.LeftSideBar
            && screen.LeftSideBarViewModel is IRoutableViewModel leftPriorViewModel)
        {
            screen.NavigationStack[routableViewModel.LayoutType].RemoveAll(item => item.GetType() == routableViewModel.GetType());
            screen.NavigationStack[routableViewModel.LayoutType].Add(leftPriorViewModel);
        }
        else if (routableViewModel.LayoutType == ViewLayoutType.Central)
        {
            if (screen.CentralViewModel is IRoutableViewModel centralPriorViewModel)
            {
                screen.NavigationStack[routableViewModel.LayoutType].RemoveAll(item => item.GetType() == routableViewModel.GetType());
                screen.NavigationStack[routableViewModel.LayoutType].Add(centralPriorViewModel);
            }
            else
            {
                centralPriorViewModel = null!;
            }

            if (screen.RightSideBarViewModel is IRoutableViewModel rightPriorViewModel
                && rightPriorViewModel.RelatedToCentral)
            {
                if (screen.RightSideBarViewModel is RoutableViewModelBase routableRightSidebar)
                {
                    routableRightSidebar.CloseView();
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
            screen.NavigationStack[routableViewModel.LayoutType].RemoveAll(item => item.GetType() == routableViewModel.GetType());
            screen.NavigationStack[routableViewModel.LayoutType].Add(rightPriorViewModel);
        }
        else
        {
            leftPriorViewModel = null!;
            rightPriorViewModel = null!;
        }

        try
        {
            screen.RoutingCancellationRequested += CancelToken;
            var token = routingCts.Token;
            token.ThrowIfCancellationRequested();
            var sw = Stopwatch.StartNew();
            var contentLoadTask = routableViewModel.ViewContentLoadAsync(token);
            if (contextViewModel != null)
            {
                // Only show loading screen if page isn't loading super quickly.
                await Task.Delay(LoadingAssetMinWaitMs, token);
                if (!contentLoadTask.IsCompleted)
                {
                    token.ThrowIfCancellationRequested();
                    var loadingAsset = AssetHelper.GetFullAssetPath("/Assets/Icons/loading.svg");
                    switch (routableViewModel.LayoutType)
                    {
                        case ViewLayoutType.LeftSideBar:
                            screen.LeftSideBarViewModel = loadingAsset;
                            break;

                        case ViewLayoutType.Central:
                            screen.CentralViewModel = loadingAsset;
                            break;

                        case ViewLayoutType.RightSideBar:
                            screen.RightSideBarViewModel = loadingAsset;
                            break;
                    }

                    await Task.Delay((int)Math.Max(0, LoadingAssetMaximumWaitMs - sw.Elapsed.TotalMilliseconds), token);
                }
            }
            await contentLoadTask;
            token.ThrowIfCancellationRequested();
            switch (routableViewModel.LayoutType)
            {
                case ViewLayoutType.LeftSideBar:
                    if (screen.LeftSideBarViewModel is IRoutableViewModel routabelLeft && routabelLeft != (IRoutableViewModel)routableViewModel)
                    {
                        routabelLeft.OnNavigatedFrom?.Invoke(routableViewModel, EventArgs.Empty);
                    }

                    routableViewModel.OnNavigatedTo?.Invoke(screen.LeftSideBarViewModel, EventArgs.Empty);

                    if (screen.LayoutMode == WindowLayout.Narrow && screen.CentralViewModel != null)
                    {
                        rememberedCentral = screen.CentralViewModel;
                        screen.CentralViewModel = null;
                    }

                    screen.LeftSideBarViewModel = routableViewModel;
                    break;

                case ViewLayoutType.Central:
                    screen.CentralViewModel = routableViewModel;
                    rememberedCentral = null;

                    if (screen.CentralViewModel is IRoutableViewModel routabelCenter && routabelCenter != (IRoutableViewModel)routableViewModel)
                    {
                        routabelCenter.OnNavigatedFrom?.Invoke(routableViewModel, EventArgs.Empty);
                    }

                    routableViewModel.OnNavigatedTo?.Invoke(screen.CentralViewModel, EventArgs.Empty);

                    if (screen.LayoutMode == WindowLayout.Narrow)
                    {
                        screen.LeftSideBarViewModel = null;
                    }
                    break;

                case ViewLayoutType.RightSideBar:
                    if (screen.RightSideBarViewModel is IRoutableViewModel routabelRight && routabelRight != (IRoutableViewModel)routableViewModel)
                    {
                        routabelRight.OnNavigatedFrom?.Invoke(routableViewModel, EventArgs.Empty);
                    }

                    routableViewModel.OnNavigatedTo?.Invoke(screen.RightSideBarViewModel, EventArgs.Empty);

                    if (screen.LayoutMode < WindowLayout.ThreeColumns && screen.CentralViewModel != null)
                    {
                        rememberedCentral = screen.CentralViewModel;
                        screen.CentralViewModel = null;
                    }

                    screen.RightSideBarViewModel = routableViewModel;
                    break;

                default:
                    throw new NotImplementedException();
            }
        }
        catch (OperationCanceledException)
        {
            if (screen.NavigationStack.Count > 0)
            {
                screen.NavigationStack[routableViewModel.LayoutType].Remove(screen.NavigationStack[routableViewModel.LayoutType][^1]);
            }
        }
        finally
        {
            screen.RoutingCancellationRequested -= CancelToken;
        }

        async void CancelToken()
        {
            await routingCts.CancelAsync();
        }
    }

    /// <summary>
    /// Показать предыдущий экран
    /// </summary>
    public static async Task<bool> BackAsync(this IMultiRoutingWindow screen, ViewLayoutType viewLayoutType)
    {
        IRoutableViewModel? backViewModel = null;
        while (screen.NavigationStack[viewLayoutType].Count > 0 && (backViewModel == null
            || backViewModel == screen.GetViewModelOutOfLayoutType(viewLayoutType)))
        {
            backViewModel = screen.NavigationStack[viewLayoutType][^1];
            screen.NavigationStack[viewLayoutType].Remove(backViewModel);
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

        for (var i = screen.NavigationStack[layoutType].Count - 1; i >= 0; i--)
        {
            var target = screen.NavigationStack[layoutType][i];
            if (type.IsAssignableFrom(target.GetType()))
            {
                // Cleanup the stack up and including the back-target.
                for (var j = i; j < screen.NavigationStack[target.LayoutType].Count; j++)
                {
                    screen.NavigationStack[target.LayoutType].RemoveAt(j);
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

                if (screen.LayoutMode < WindowLayout.ThreeColumns && rememberedCentral != null)
                {
                    screen.CentralViewModel = rememberedCentral;
                    rememberedCentral = null;
                }

                break;
        }
    }

    private static void ResetRelated(IMultiRoutingWindow screen)
    {
        if (screen.RightSideBarViewModel is IRoutableViewModel rightPriorViewModel
            && rightPriorViewModel.RelatedToCentral)
        {
            screen.RightSideBarViewModel = null;
            rememberedRightSideBar = null;
        }
    }

    /// <summary>
    /// Расположить по layout страницы
    /// </summary>
    public static void ArrangeLayout(this IMultiRoutingWindow screen)
    {
        switch (screen.LayoutMode)
        {
            case WindowLayout.ThreeColumns:
                if (rememberedRightSideBar != null)
                {
                    screen.RightSideBarViewModel = rememberedRightSideBar;
                }

                rememberedRightSideBar = null;

                if (rememberedLeftSideBar != null)
                {
                    screen.LeftSideBarViewModel = rememberedLeftSideBar;
                }

                rememberedLeftSideBar = null;

                if (rememberedCentral != null)
                {
                    screen.CentralViewModel = rememberedCentral;
                }

                rememberedCentral = null;
                break;

            case WindowLayout.TwoColumns:
                if (screen.RightSideBarViewModel != null)
                {
                    rememberedRightSideBar = screen.RightSideBarViewModel;
                }

                screen.RightSideBarViewModel = null;

                if (rememberedLeftSideBar != null)
                {
                    screen.LeftSideBarViewModel = rememberedLeftSideBar;
                }

                rememberedLeftSideBar = null;

                if (rememberedCentral != null)
                {
                    screen.CentralViewModel = rememberedCentral;
                }

                rememberedCentral = null;
                break;

            case WindowLayout.Narrow:
                if (screen.RightSideBarViewModel != null)
                {
                    rememberedRightSideBar = screen.RightSideBarViewModel;
                }

                screen.RightSideBarViewModel = null;

                if (screen.LeftSideBarViewModel != null)
                {
                    rememberedLeftSideBar = screen.LeftSideBarViewModel;
                }

                screen.LeftSideBarViewModel = null;

                if (rememberedCentral != null)
                {
                    screen.CentralViewModel = rememberedCentral;
                }

                rememberedCentral = null;
                break;
        }
    }
}
