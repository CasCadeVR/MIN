namespace MIN.Core.Messaging.Contracts.Interfaces;

/// <summary>
/// Сообщение, которое может являться как ответом на другое сообщение
/// </summary>
public interface IReplyable
{
    /// <summary>
    /// Идентификатор сообщения, на которое дан ответ
    /// </summary>
    Guid? ReplyToMessageId { get; set; }
}
