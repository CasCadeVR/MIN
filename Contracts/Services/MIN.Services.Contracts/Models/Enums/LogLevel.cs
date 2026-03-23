namespace MIN.Services.Contracts.Models.Enums
{
    /// <summary>
    /// Уровень серъёзности сообщения в логах
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// Информация
        /// </summary>
        Information = 0,

        /// <summary>
        /// Предупреждение
        /// </summary>
        Warning = 1,

        /// <summary>
        /// Ошибка
        /// </summary>
        Error = 2,
    }
}
