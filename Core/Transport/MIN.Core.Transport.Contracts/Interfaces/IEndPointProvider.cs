namespace MIN.Core.Transport.Contracts.Interfaces;

/// <summary>
/// Помощник Bootstrap для создания конечных точек для комнат на основе текущей конфигурации транспорта.
/// </summary>
public interface IEndPointProvider
{
    /// <summary>
    /// Создает конечную точку для данной комнаты, используя текущий тип транспорта.
    /// </summary>
    IEndpoint CreateEndpointForRoom(Guid roomId);
}
