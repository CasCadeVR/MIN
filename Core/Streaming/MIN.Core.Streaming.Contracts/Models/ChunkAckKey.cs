namespace MIN.Core.Streaming.Contracts.Models;

/// <summary>
/// Ключ идентификации пакета среди других приходящих пакетов
/// </summary>
/// <param name="StreamId">Идентификатор потока</param>
/// <param name="ChunkIndex">Индекс пакета</param>
public readonly record struct ChunkAckKey(Guid StreamId, int ChunkIndex);
