using MIN.Sessions.Core.Services.Contracts.Interfaces;

namespace MIN.Sessions.Core.DI.FeatureCollection;

/// <summary>
/// Набор функциональностей для Session
/// </summary>
public interface ISessionFeatureCollection
{
    /// Список <inheritdoc cref="ISessionPresenter"/>
    IEnumerable<ISessionPresenter> SessionPresenters { get; }
}
