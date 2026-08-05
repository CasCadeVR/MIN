namespace MIN.Core.Services.Contracts.Interfaces.Pipeline;

/// <summary>
/// Обработчик подтверждений получения пакетов потоков (ACK)
/// </summary>
public interface IAckHandler
{
    /// <summary>
    /// Являются ли данные подтверждением пакета потока
    /// </summary>
    bool CanHandle(byte[] data);

    /// <summary>
    /// Обработать подтверждение пакета потока
    /// </summary>
    void Handle(byte[] data);
}
