using MIN.Services.Contracts.Models.Enums;

namespace MIN.Services.Contracts.Models;

/// <summary>
/// Сообщения в чате
/// </summary>
public class ChatMessage
{
    /// <summary>
    /// Идентификатор сообщения
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Имя отправителя сообщения
    /// </summary>
    public string SenderName { get; set; } = string.Empty;

    /// <summary>
    /// Имя компьютера отправителя сообщения
    /// </summary>
    public string SenderPCName { get; set; } = string.Empty;

    /// <summary>
    /// Время написания сообщения
    /// </summary>
    public TimeOnly Time { get; set; } = TimeOnly.FromDateTime(DateTime.UtcNow.ToLocalTime());

    /// <summary>
    /// Штамп текущего времени для синхронизации
    /// </summary>
    public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Тип сообщения (текст, файл)
    /// </summary>
    public MessageType MessageType { get; set; }

    /// <summary>
    /// Само сообщение
    /// </summary>
    public string Content { get; set; } = string.Empty;
}
