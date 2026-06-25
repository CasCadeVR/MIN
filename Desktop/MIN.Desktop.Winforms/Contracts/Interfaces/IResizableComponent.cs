namespace MIN.Desktop.Contracts.Interfaces;

/// <summary>
/// Компонент, нуждающийся в перерисовки, исходя из его содержимого
/// </summary>
public interface IResizableComponent
{
    /// <summary>
    /// Подстроивает размеры сообщений под содержимое внутри и возвращает полученную высоту
    /// </summary>
    /// <returns>
    /// Вычисленную высоту, исходя из содержимого
    /// </returns>
    int ResizeOutOfPrefferedSize();

    /// <summary>
    /// Попросить компонент, содержащй текущий компонет перерасчитать его высоту
    /// </summary>
    Action? AskParentForResize { get; set; }
}
