namespace MIN.Core.Messaging.Contracts.Interfaces;

/// <summary>
/// Сообщение, которое имеет текстовое поле и может быть изменено
/// </summary>
public interface IContentEditable
{
    /// <summary>
    /// Содержимое сообщения
    /// </summary>
    string Content { get; set; }

    /// <summary>
    /// Было ли уже изменено сообщение
    /// </summary>
    bool IsEdited { get; set; }

    /// <summary>
    /// Когда было последнее изменение
    /// </summary>
    DateTime EditedAt { get; set; }

    /// <summary>
    /// Отредактировать
    /// </summary>
    public void Edit(IContentEditable newContent);
}
