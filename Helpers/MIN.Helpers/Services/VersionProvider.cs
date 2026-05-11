using MIN.Helpers.Contracts.Interfaces;

namespace MIN.Helpers.Services;

/// <inheritdoc cref="IVersionProvider"/>
public class VersionProvider : IVersionProvider
{
    /// <inheritdoc />
    public Version Version { get; init; } = null!;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="VersionProvider"/>
    /// </summary>
    public VersionProvider(Version version)
    {
        Version = version;
    }
}
