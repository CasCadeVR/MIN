namespace MIN.Desktop.Contracts;

/// <summary>
/// Схема шрифтов приложения
/// </summary>
public static class FontScheme
{
    /// <summary>
    /// Основной шрифт приложения
    /// </summary>
    public readonly static Font Default = new Font("Segoe UI", 9.75f, FontStyle.Regular);

    /// <summary>
    /// Заголовки
    /// </summary>
    public readonly static Font Heading1 = new Font("Segoe UI", 16f, FontStyle.Bold);
    public readonly static Font Heading2 = new Font("Segoe UI", 14f, FontStyle.Bold);
    public readonly static Font Heading3 = new Font("Segoe UI", 12f, FontStyle.Bold);

    /// <summary>
    /// Мелкий текст (подсказки, подписи)
    /// </summary>
    public readonly static Font Caption = new Font("Segoe UI", 8f, FontStyle.Regular);

    /// <summary>
    /// Очень мелкий текст
    /// </summary>
    public readonly static Font MicroCaption = new Font("Segoe UI", 7f, FontStyle.Regular);

    /// <summary>
    /// Моноширинный (для кода, номеров, идентификаторов)
    /// </summary>
    public readonly static Font Monospace = new Font("Lucida Sans Typewriter", 10f, FontStyle.Regular, GraphicsUnit.Point);

    /// <summary>
    /// Крупный акцентный (для кнопок, важных надписей)
    /// </summary>
    public readonly static Font Emphasis = new Font("Segoe UI", 10.5f, FontStyle.Bold);
}
