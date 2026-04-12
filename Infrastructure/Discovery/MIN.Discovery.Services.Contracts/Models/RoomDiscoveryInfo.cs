using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Transport.Contracts.Interfaces;

namespace MIN.Discovery.Services.Contracts.Models;

/// <summary>
/// Информация о найденной комнате
/// </summary>
public sealed class RoomDiscoveryInfo
{
    /// <summary>
    /// Информация о комнате
    /// </summary>
    public RoomInfo Room { get; set; } = null!;

    /// <summary>
    /// Точка подключения к комнате
    /// </summary>
    public IEndpoint Endpoint { get; set; } = null!;
}
