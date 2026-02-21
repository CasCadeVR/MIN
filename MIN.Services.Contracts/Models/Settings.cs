using MIN.Services.Contracts.Models.Enums;

namespace MIN.Services.Contracts.Models
{
    public class Settings
    {
        /// <summary>
        /// Время ожиждания поиска комнаты
        /// </summary>
        public int DiscoveryTimeout { get; set; } = 1000;

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