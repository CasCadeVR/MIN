namespace MIN.Services.Services
{
    /// <summary>
    /// Сервис для предоставления имени pipe
    /// </summary>
    public static class PipeNameProvider
    {
        private const string pipePrefix = "MIN_Chat_";

        /// <summary>
        /// Генерация имени канала для комнаты по её уникальному идентификатору
        /// </summary>
        public static string GetRoomPipeName(Guid roomId) =>
            $"{pipePrefix}{roomId:N}";

        /// <summary>
        /// Валидация имени (защита от инъекций в имя канала)
        /// </summary>
        public static bool IsValidPipeName(string name) =>
            !string.IsNullOrWhiteSpace(name) &&
            name.Length <= 256 &&
            !name.Any(c => char.IsControl(c) || c == '\\' || c == '/');
    }
}
