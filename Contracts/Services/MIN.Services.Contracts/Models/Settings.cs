using MIN.Helpers.Contracts.Models.Enums;

namespace MIN.Helpers.Contracts.Models
{
    /// <summary>
    /// Настройки
    /// </summary>
    public class Settings
    {
        /// <summary>
        /// Время ожидания поиска комнаты
        /// </summary>
        public int DiscoveryTimeout { get; set; } = 300;

        /// <summary>
        /// Метод поиска комнат
        /// </summary>
        public SearchMethod SearchMethod { get; set; } = SearchMethod.ClassRoom;

        /// <summary>
        /// Избранные компьютеры
        /// </summary>
        public IEnumerable<string> PreferredPCNames { get; set; } = [];
    }
}
