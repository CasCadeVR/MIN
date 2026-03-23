using MIN.Services.Updates.Contracts.Models;

namespace MIN.Services.Updates.Contracts.Interfaces;

/// <summary>
/// Сервис по обновлениям
/// </summary>
public interface IUpdateService
{
    /// <summary>
    /// Проверить последние обновления
    /// </summary>
    Task<UpdateCheckResult> CheckForUpdatesAsync(string owner, string repo, CancellationToken cancellationToken);
}
