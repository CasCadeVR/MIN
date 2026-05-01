namespace MIN.Desktop.Contracts.Models;

/// <summary>
/// Приложение файла
/// </summary>
/// <param name="FileName">Имя файла</param>
/// <param name="FilePath">Путь к файлу</param>
public record struct FileAttachment(string FileName, string FilePath) { }
