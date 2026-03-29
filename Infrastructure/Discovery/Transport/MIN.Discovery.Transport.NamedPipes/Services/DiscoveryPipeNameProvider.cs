namespace MIN.Discovery.Transport.NamedPipes.Services;

/// <summary>
/// Поставщик имён каналов для обнаружения другими ПК 
/// </summary>
public static class DiscoveryPipeNameProvider
{
    /// <summary>
    /// Discovery канал для каждого ПК
    /// </summary>
    public static string GetDiscoveryPipeName(string pcName) =>
        $"MIN_Discovery_{SanitizeMachineName(pcName)}";

    private static string SanitizeMachineName(string pcName)
    {
        var cleaned = string.Concat(pcName.Where(c =>
            char.IsLetterOrDigit(c) || c == '_' || c == '.'));

        var trimmed = cleaned.Trim('_');

        var length = Math.Min(trimmed.Length, 100);
        return trimmed.Substring(0, length);
    }
}
