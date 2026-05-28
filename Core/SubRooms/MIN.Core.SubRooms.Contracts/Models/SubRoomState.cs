namespace MIN.Core.SubRooms.Contracts.Models;

/// <summary>
/// Словарь подкомнат с подсчётом id 
/// </summary>
public sealed class SubRoomState
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
