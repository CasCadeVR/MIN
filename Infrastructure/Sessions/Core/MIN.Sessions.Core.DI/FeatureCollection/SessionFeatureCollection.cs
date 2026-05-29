using MIN.Sessions.Core.Services.Contracts.Interfaces;

namespace MIN.Sessions.Core.DI.FeatureCollection;

/// <inheritdoc cref="ISessionFeatureCollection"/>
public class SessionFeatureCollection : ISessionFeatureCollection
{
    /// Список <inheritdoc cref="ISessionPresenter"/>
    public IEnumerable<ISessionPresenter> SessionPresenters { get; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="SessionFeatureCollection"/>
    /// </summary>
    public SessionFeatureCollection(IEnumerable<ISessionPresenter> sessionPresenters)
    {
        SessionPresenters = sessionPresenters;
    }
}
