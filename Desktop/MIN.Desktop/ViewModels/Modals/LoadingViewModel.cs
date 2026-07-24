using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Input;
using MIN.Core.Entities;
using MIN.Core.Events.Contracts;
using MIN.Core.Events.Events;
using MIN.Desktop.Contracts.Enums;
using MIN.Desktop.Infrastructure.Services;
using MIN.Desktop.ViewModels.Base;

namespace MIN.Desktop.ViewModels.Modals;

/// <summary>
/// Модель окна создания комнаты
/// </summary>
public partial class LoadingViewModel : ModalViewModelBase
{
    private readonly IEventBus eventBus;
    private System.Timers.Timer timeoutTimer = null!;
    private Action<Room?> onRoomReady = null!;
    private CancellationTokenSource cts = null!;

    private HashSet<IDisposable> eventTokens = null!;
    private bool gotRoom;

    /// <summary>
    /// Идентификатор комнаты
    /// </summary>
    /// <remarks>
    /// null, если ещё не прошёл этап протокола
    /// </remarks>
    public Guid? RoomId { get; set; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="LoadingViewModel"/>
    /// </summary>
    public LoadingViewModel(IEventBus eventBus)
    {
        this.eventBus = eventBus;

        if (!Design.IsDesignMode)
        {
            SubscribeToEvents();
        }
    }

    /// <summary>
    /// Подгрузить данные о комнате и перезагрузить
    /// </summary>
    public async Task LoadRoomDataAndRefresh(Action<Room?> onRoomReady,
        CancellationTokenSource cts,
        int timeoutMs = 10000)
    {
        timeoutTimer = new System.Timers.Timer { Interval = timeoutMs };
        timeoutTimer.Elapsed += OnTimeout;
        timeoutTimer.Start();

        this.onRoomReady = onRoomReady;
        this.cts = cts;
    }

    private void OnTimeout(object? sender, EventArgs e)
    {
        timeoutTimer.Stop();
        onRoomReady.Invoke(null!);

        Dispatcher.UIThread.Post(() =>
        {
            CloseByCode();
            InAppNotifier.Warning("Не удалось подключиться: Время подключения истекло.\nВозможно, комнаты уже и нет");
        });
    }

    private void SubscribeToEvents()
    {
        eventTokens = [
            eventBus.Subscribe<RoomStateChangedEvent>(OnRoomStateChangedEventReceived),
            eventBus.Subscribe<ErrorOccurredEvent>(OnErrorOccured),
        ];
    }

    private async Task OnErrorOccured(ErrorOccurredEvent eventMessage, CancellationToken cancellationToken)
    {
        if (eventMessage.RoomId != RoomId)
        {
            return;
        }

        onRoomReady.Invoke(null!);
        Dispatcher.UIThread.Post(() =>
        {
            CloseByCode();
            InAppNotifier.Error(eventMessage.ErrorMessage);
        });
    }

    private async Task OnRoomStateChangedEventReceived(RoomStateChangedEvent eventMessage, CancellationToken cancellationToken)
    {
        if (eventMessage.Room.Id != RoomId)
        {
            return;
        }

        gotRoom = true;
        Dispatcher.UIThread.Post(() =>
        {
            CloseByCode(ButtonOptions.Ok);
            InAppNotifier.Success($"Подключение к комнате {eventMessage.Room.Name} прошло успешно!");
        });
        onRoomReady.Invoke(eventMessage.Room);
    }

    /// <summary>
    /// Остановить загрузку
    /// </summary>
    [RelayCommand]
    public void StopLoading()
    {
        foreach (var token in eventTokens)
        {
            token.Dispose();
        }

        if (!gotRoom)
        {
            cts.Cancel();
        }

        timeoutTimer.Stop();
        timeoutTimer.Dispose();
    }
}
