namespace MIN.Desktop.Contracts.Schemes;

/// <summary>
/// Схема шрифтов приложения
/// </summary>
public static class FontScheme
{
    /// <summary>
    /// Основной шрифт приложения
    /// </summary>
    public readonly static Font Default = new("Segoe UI", 9.75f, FontStyle.Regular);

    /// <summary>
    /// Заголовок 1 уровня
    /// </summary>
    public readonly static Font Heading1 = new("Segoe UI", 16f, FontStyle.Bold);

    /// <summary>
    /// Заголовок 2 уровня
    /// </summary>
    public readonly static Font Heading2 = new("Segoe UI", 14f, FontStyle.Bold);

    /// <summary>
    /// Заголовок 3 уровня
    /// </summary>
    public readonly static Font Heading3 = new("Segoe UI", 12f, FontStyle.Bold);

    /// <summary>
    /// Мелкий текст (подсказки, подписи)
    /// </summary>
    public readonly static Font Caption = new("Segoe UI", 8f, FontStyle.Regular);

    /// <summary>
    /// Очень мелкий текст
    /// </summary>
    public readonly static Font MicroCaption = new("Segoe UI", 7f, FontStyle.Regular);

    /// <summary>
    /// Моноширинный (для кода, номеров, идентификаторов)
    /// </summary>
    public readonly static Font Monospace = new("Lucida Sans Typewriter", 10f, FontStyle.Regular, GraphicsUnit.Point);

    /// <summary>
    /// Крупный акцентный (для кнопок, важных надписей)
    /// </summary>
    public readonly static Font Emphasis = new("Segoe UI", 10.5f, FontStyle.Bold);
}
