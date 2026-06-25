using MIN.Common.Core.Contracts.Interfaces;

namespace MIN.Desktop.Contracts.Interfaces;

/// <summary>
/// Статус для отображения в UI
/// </summary>
public interface IDescribableStatus : IDescribable
{
    /// <summary>
    /// Идентификатор статуса
    /// </summary>
    Guid Id { get; }
}
