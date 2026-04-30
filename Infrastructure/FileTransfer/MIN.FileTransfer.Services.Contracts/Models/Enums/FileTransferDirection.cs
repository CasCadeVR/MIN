namespace MIN.FileTransfer.Services.Contracts.Models.Enums;

/// <summary>
/// Направление передачи файла
/// </summary>
public enum FileTransferDirection
{
    /// <summary>
    /// Выгрузить файл на сервер
    /// </summary>
    Upload,

    /// <summary>
    /// Загрузить файл с сервера
    /// </summary>
    Download,
}
