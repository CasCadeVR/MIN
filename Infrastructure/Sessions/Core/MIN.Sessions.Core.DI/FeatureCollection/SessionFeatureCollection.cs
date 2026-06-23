using MIN.Sessions.Core.Services.Contracts.Interfaces;

namespace MIN.Sessions.Core.DI.FeatureCollection;

/// <inheritdoc cref="ISessionFeatureCollection"/>
public class SessionFeatureCollection : ISessionFeatureCollection
{
    /// <inheritdoc cref="ISessionScanner"/>
    public ISessionScanner SessionScanner { get; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="SessionFeatureCollection"/>
    /// </summary>
    public SessionFeatureCollection(ISessionScanner sessionScanner)
    {
        SessionScanner = sessionScanner;
    }
}
