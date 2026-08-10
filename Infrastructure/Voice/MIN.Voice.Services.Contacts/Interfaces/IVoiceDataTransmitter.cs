namespace MIN.Voice.Services.Contacts.Interfaces;

/// <summary>
/// Сервис передачи звука
/// </summary>
public interface IVoiceDataTransmitter : IDisposable
{
    /// <summary>
    /// Начать передачу звука
    /// </summary>
    void Begin(Guid roomId, int subRoomId);

    /// <summary>
    /// Остановить передачу звука
    /// </summary>
    void End();
}
