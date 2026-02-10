namespace MIN.Desktop.Contracts;

/// <summary>
/// Цветовая схема приложения
/// </summary>
public static class ColorScheme
{
    /// <summary>
    /// Цвет заднего фона приложения (основной фон)
    /// </summary>
    public static readonly Color FormBackground = ColorTranslator.FromHtml("#F8F9FF");

    /// <summary>
    /// Цвет заднего фона главной панели (список чатов / боковая панель)
    /// </summary>
    public static readonly Color MainPanelBackground = ColorTranslator.FromHtml("#F0F2FF");

    /// <summary>
    /// Цвет заднего фона области чата
    /// </summary>
    public static readonly Color ChatAreaBackground = ColorTranslator.FromHtml("#FFFFFF");

    /// <summary>
    /// Цвет фона входящего сообщения
    /// </summary>
    public static readonly Color IncomingMessageBackground = ColorTranslator.FromHtml("#F0F2FF");

    /// <summary>
    /// Цвет фона исходящего сообщения
    /// </summary>
    public static readonly Color OutgoingMessageBackground = ColorTranslator.FromHtml("#E6E8FF");

    /// <summary>
    /// Основной акцентный цвет (фиолетовый — для кнопок, выделений, активных элементов)
    /// </summary>
    public static readonly Color PrimaryAccent = ColorTranslator.FromHtml("#6A5BFF");

    /// <summary>
    /// Вторичный акцентный цвет (светло-фиолетовый — для ховеров, второстепенных действий)
    /// </summary>
    public static readonly Color SecondaryAccent = ColorTranslator.FromHtml("#A79DFF");

    /// <summary>
    /// Цвет текста по умолчанию (основной контент)
    /// </summary>
    public static readonly Color TextPrimary = ColorTranslator.FromHtml("#2D2B3A");

    /// <summary>
    /// Цвет второстепенного текста (время, метаданные, подписи)
    /// </summary>
    public static readonly Color TextSecondary = ColorTranslator.FromHtml("#7A778F");

    /// <summary>
    /// Цвет текста на акцентном фоне (например, белый текст на кнопке)
    /// </summary>
    public static readonly Color TextOnAccent = ColorTranslator.FromHtml("#FFFFFF");

    /// <summary>
    /// Цвет разделителей и лёгких границ
    /// </summary>
    public static readonly Color DividerColor = ColorTranslator.FromHtml("#E4E6F0");

    /// <summary>
    /// Цвет фона поля ввода сообщения
    /// </summary>
    public static readonly Color InputFieldBackground = ColorTranslator.FromHtml("#F8F9FF");

    /// <summary>
    /// Цвет ошибки или предупреждения (например, недоставленное сообщение)
    /// </summary>
    public static readonly Color ErrorColor = ColorTranslator.FromHtml("#FF6B6B");
}