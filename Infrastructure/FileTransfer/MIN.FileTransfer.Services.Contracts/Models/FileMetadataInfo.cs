namespace MIN.FileTransfer.Services.Contracts.Models;

/// <summary>
/// Информация о файле, зарегистрированном по Id сообщения метаданных
/// </summary>
public sealed class FileMetadataInfo
{
    /// <summary>
    /// Id сообщения метаданных (FileMetadataMessage.Id)
    /// </summary>
    public Guid FileMetadataId { get; init; }

    /// <summary>
    /// Id комнаты
    /// </summary>
    public Guid RoomId { get; init; }

    /// <summary>
    /// Имя файла
    /// </summary>
    public string FileName { get; init; } = string.Empty;

    /// <summary>
    /// Оригинальный путь к файлу (если отправитель — хост)
    /// </summary>
    public string? OriginalPath { get; init; }

    /// <summary>
    /// Файл уже сохранён в RoomFiles
    /// </summary>
    public bool IsStoredOnServer { get; init; }
}
