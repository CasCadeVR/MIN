using MIN.Core.SubRooms.Contracts.Models;

namespace MIN.Core.SubRooms.Models;

internal sealed class SubRoomState
{
    /// <summary>
    /// Словарь подкомнат
    /// </summary>
    public readonly Dictionary<int, SubRoomInfo> SubRooms = [];

    /// <summary>
    /// Идентификатор следующей добавленной подкомнаты
    /// </summary>
    public int NextId = 1;
}
