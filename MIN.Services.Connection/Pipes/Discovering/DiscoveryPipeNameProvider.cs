namespace MIN.Services.Connection.Pipes.Discovering
{
    /// <summary>
    /// Поставщик имён каналов для обнаружения другими пк 
    /// </summary>
    public static class DiscoveryPipeNameProvider
    {
        /// <summary>
        /// Discovery канал для каждого ПК
        /// </summary>
        public static string GetDiscoveryPipeName(string pcName) =>
            $"MIN_Discovery_{SanitizePCName(pcName)}";

        private static string SanitizePCName(string pcName)
        {
            var cleaned = string.Concat(pcName.Where(c =>
                char.IsLetterOrDigit(c) || c == '_' || c == '.'));

            var trimmed = cleaned.Trim('_');

            var length = Math.Min(trimmed.Length, 100);
            return trimmed.Substring(0, length);
        }
    }
}
