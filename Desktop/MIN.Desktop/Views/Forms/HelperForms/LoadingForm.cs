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
    private readonly Action<Room?> onRoomReady;
    private readonly SynchronizationContext uiContext;
    private readonly System.Windows.Forms.Timer timeoutTimer;
    private readonly CancellationTokenSource cts;

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
    /// Инициализирует новый экземпляр <see cref="LoadingForm"/>
    /// </summary>
    public LoadingForm(IEventBus eventBus, Action<Room?> onRoomReady, CancellationTokenSource cts, int timeoutMs = 10000)
    {
        InitializeComponent();

        uiContext = SynchronizationContext.Current
            ?? throw new InvalidOperationException("Must be created on UI thread");

        this.onRoomReady = onRoomReady;
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
            onRoomReady.Invoke(null!);
            Close();
            MessageBox.Show("Не удалось подключиться: Время подключения истекло.\nВозможно, комнаты уже и нет", "Ошибка",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }, null);
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

        uiContext.Post(_ =>
        {
            onRoomReady.Invoke(null!);
            Close();
            MessageBox.Show(eventMessage.ErrorMessage, "Ошибка",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }, this);

        await Task.CompletedTask;
    }

    private async Task OnRoomStateChangedEventReceived(RoomStateChangedEvent eventMessage, CancellationToken cancellationToken)
    {
        if (eventMessage.Room.Id != RoomId)
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
