using MIN.Core.Messaging.Contracts.Interfaces;

namespace MIN.Core.Messaging.Contracts.Messages;

/// <summary>
/// Базовый класс для сообщений, имеющих текстовое представление и могут быть отредактированы
/// </summary>
public abstract class BaseContentMessage : BaseMessage, IContentEditable
{
    /// <inheritdoc />
    public string Content { get; set; } = string.Empty;

    /// <inheritdoc />
    public bool IsEdited { get; set; }

    /// <inheritdoc />
    public DateTime EditedAt { get; set; }

    /// <inheritdoc />
    public void Edit(IContentEditable newContent)
    {
        Content = newContent.Content;
        IsEdited = newContent.IsEdited;
        EditedAt = newContent.EditedAt;
    }
}
