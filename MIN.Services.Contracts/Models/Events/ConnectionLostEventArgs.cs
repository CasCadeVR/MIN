namespace MIN.Services.Contracts.Models.Events;

/// <summary>
/// Аргументы события получения сообщения
/// </summary>
public class ConnectionLostEventArgs : EventArgs
{
    /// <summary>
    /// Причниа потери связи
    /// </summary>
    public string Reason { get; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="ConnectionLostEventArgs"/>
    /// </summary>
    public ConnectionLostEventArgs(string reason)
    {
        Reason = reason ?? throw new ArgumentNullException(nameof(reason));
    }
}
