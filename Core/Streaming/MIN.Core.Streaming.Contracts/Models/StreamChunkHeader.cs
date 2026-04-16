namespace MIN.Core.Streaming.Contracts.Models
{
    /// <summary>
    /// Заголовок пакета потока
    /// </summary>
    /// <param name="Flags">Флаги</param>
    /// <param name="StreamId">Идентикатор потока</param>
    /// <param name="Index">Номер пакета</param>
    /// <param name="Total">Всего пакетов</param>
    public readonly record struct StreamChunkHeader(
        StreamChunkFlags Flags,
        Guid StreamId,
        int Index,
        int Total);
}
