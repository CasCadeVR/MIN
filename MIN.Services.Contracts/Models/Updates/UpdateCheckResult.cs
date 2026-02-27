namespace MIN.Services.Contracts.Models.Updates
{
    /// <summary>
    /// Модель для получения данных обновления
    /// </summary>
    public class UpdateCheckResult
    {
        /// <summary>
        /// Доступно ли обновление
        /// </summary>
        public bool IsUpdateAvailable { get; set; }

        /// <summary>
        /// Последняя версия
        /// </summary>
        public string? LatestVersion { get; set; }

        /// <summary>
        /// Ссылка на релиз
        /// </summary>
        public string? ReleaseUrl { get; set; }

        /// <summary>
        /// Новости
        /// </summary>
        public string? ReleaseNotes { get; set; }
    }
}
