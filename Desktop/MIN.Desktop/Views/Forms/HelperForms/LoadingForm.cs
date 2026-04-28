using MIN.Core.Entities;
using MIN.Core.Events.Contracts;
using MIN.Core.Events.Events;
using MIN.Desktop.Contracts.Views.Forms;

namespace MIN.Desktop.Views.Forms.HelperForms;

/// <summary>
/// Форма загрузки
/// </summary>
public partial class LoadingForm : StyledForm
{
    private readonly IEventBus eventBus;
    private readonly Guid roomId;
    private readonly Action<Room?> onRoomReady;
    private readonly SynchronizationContext uiContext;
    private readonly System.Windows.Forms.Timer timeoutTimer;
    private readonly CancellationTokenSource cts;

    private HashSet<IDisposable> eventTokens = null!;

    private bool gotRoom;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="LoadingForm"/>
    /// </summary>
    public LoadingForm(Guid roomId, IEventBus eventBus, Action<Room?> onRoomReady, CancellationTokenSource cts, int timeoutMs = 10000)
    {
        InitializeComponent();

        uiContext = SynchronizationContext.Current
            ?? throw new InvalidOperationException("Must be created on UI thread");

        this.onRoomReady = onRoomReady;
        this.roomId = roomId;
        this.eventBus = eventBus;
        this.cts = cts;

        timeoutTimer = new System.Windows.Forms.Timer { Interval = timeoutMs };
        timeoutTimer.Tick += OnTimeout;
        timeoutTimer.Start();

        SubscribeToEvents();
    }

    private void OnTimeout(object? sender, EventArgs e)
    {
        timeoutTimer.Stop();
        uiContext.Post(_ =>
        {
            MessageBox.Show("Не удалось подключиться: Время подключения истекло.\nВозможно, комнаты уже и нет", "Ошибка",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            onRoomReady.Invoke(null!);
            Close();
        }, null);
    }

    private void SubscribeToEvents()
    {
        eventTokens = [
            eventBus.Subscribe<RoomStateChangedEvent>(OnRoomStateChangedEventReceived),
            eventBus.Subscribe<ErrorOccurredEvent>(OnErrorOccurredEvent),
        ];
    }

    private async Task OnErrorOccurredEvent(ErrorOccurredEvent eventMessage, CancellationToken cancellationToken)
    {
        if (eventMessage.RoomId != roomId)
        {
            return;
        }

        uiContext.Post(_ =>
        {
            MessageBox.Show(eventMessage.ErrorMessage, "Ошибка",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            onRoomReady.Invoke(null!);
            Close();
        }, this);

        await Task.CompletedTask;
    }

    private async Task OnRoomStateChangedEventReceived(RoomStateChangedEvent eventMessage, CancellationToken cancellationToken)
    {
        if (eventMessage.Room.Id != roomId)
        {
            return;
        }

        uiContext.Post(_ =>
        {
            gotRoom = true;
            onRoomReady.Invoke(eventMessage.Room);
            Close();
        }, this);

        await Task.CompletedTask;
    }

    private void LoadingForm_FormClosing(object sender, FormClosingEventArgs e)
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
