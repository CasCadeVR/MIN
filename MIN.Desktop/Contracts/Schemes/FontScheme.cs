namespace MIN.Desktop.Contracts;

/// <summary>
/// Схема шрифтов приложения
/// </summary>
public static class FontScheme
{
    /// <summary>
    /// Основной шрифт приложения
    /// </summary>
    public static readonly Font Default = new Font("Segoe UI", 9.75f, FontStyle.Regular);

    /// <summary>
    /// Заголовки
    /// </summary>
    public static readonly Font Heading1 = new Font("Segoe UI", 16f, FontStyle.Bold);
    public static readonly Font Heading2 = new Font("Segoe UI", 14f, FontStyle.Bold);
    public static readonly Font Heading3 = new Font("Segoe UI", 12f, FontStyle.Bold);

    /// <summary>
    /// Мелкий текст (подсказки, подписи)
    /// </summary>
    public static readonly Font Caption = new Font("Segoe UI", 8.25f, FontStyle.Regular);

    /// <summary>
    /// Моноширинный (для кода, номеров, идентификаторов)
    /// </summary>
    public static readonly Font Monospace = new Font("Consolas", 9.75f, FontStyle.Regular);

    /// <summary>
    /// Крупный акцентный (для кнопок, важных надписей)
    /// </summary>
    public static readonly Font Emphasis = new Font("Segoe UI", 10.5f, FontStyle.Bold);
}