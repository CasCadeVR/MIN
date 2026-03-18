using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIN.Services.Contracts.Models
{
    /// <summary>
    /// Модель задачи поиска комнаты
    /// </summary>
    public class RoomDiscoveringTask
    {
        /// <summary>
        /// Имя компьютера комнаты для поиска
        /// </summary>
        public string PcName { get; set; } = string.Empty;

        /// <summary>
        /// Задача поиска комнаты
        /// </summary>
        public Task<DiscoveredRoom?> Task { get; set; }
    }
}
