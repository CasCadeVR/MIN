namespace MIN.Desktop.Contracts.Interfaces;

/// <summary>
/// Провайдер Cts для приложения
/// </summary>
public interface ICtsProvider
{
    /// <summary>
    /// Источник токена отмены для приложения
    /// </summary>
    CancellationTokenSource AppCts { get; }
}
