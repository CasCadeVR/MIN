using MIN.Common.Core.Contracts.Interfaces;
using MIN.Desktop.Contracts.Interfaces;

namespace MIN.Desktop.Contracts.Models;

/// <summary>
/// UI Статус отправки файла на сервер
/// </summary>
public readonly record struct FileUploadingStatus(Guid TransferId, string FileName, string SenderName, string FileSize) : IDescribableStatus
{
    /// <inheritdoc />
    public readonly Guid Id => TransferId;

    /// <inheritdoc />
    string IDescribable.GetDescription() => $"{SenderName} отправляет файл {FileName} {FileSize}";
}
