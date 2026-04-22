namespace MIN.Desktop.Contracts.Schemes;

/// <summary>
/// Цветовая схема приложения
/// </summary>
public static class ColorScheme
{
    /// <summary>
    /// Цвет заднего фона приложения (основной фон)
    /// </summary>
    public readonly static Color FormBackground = ColorTranslator.FromHtml("#F8F9FF");

    /// <summary>
    /// Цвет заднего фона главной панели (список чатов / боковая панель)
    /// </summary>
    public readonly static Color MainPanelBackground = ColorTranslator.FromHtml("#F0F2FF");

    /// <summary>
    /// Цвет заднего фона области чата
    /// </summary>
    public readonly static Color ChatAreaBackground = ColorTranslator.FromHtml("#FFFFFF");

    /// <summary>
    /// Цвет фона входящего сообщения
    /// </summary>
    public readonly static Color IncomingMessageBackground = ColorTranslator.FromHtml("#F0F2FF");

    /// <summary>
    /// Цвет фона исходящего сообщения
    /// </summary>
    public readonly static Color OutgoingMessageBackground = ColorTranslator.FromHtml("#E6E8FF");

    /// <summary>
    /// Цвет фона карточки участника в комнате
    /// </summary>
    public readonly static Color DefaultParticipantCardBackground = ColorTranslator.FromHtml("#F0F2FF");

    /// <summary>
    /// Цвет фона карточки участника в комнате, при приватном общении
    /// </summary>
    public readonly static Color PrivateParticipantCardBackground = ColorTranslator.FromHtml("#CDC0ED");

    /// <summary>
    /// Основной акцентный цвет (фиолетовый — для кнопок, выделений, активных элементов)
    /// </summary>
    public readonly static Color PrimaryAccent = ColorTranslator.FromHtml("#6A5BFF");

    /// <summary>
    /// Вторичный акцентный цвет (светло-фиолетовый — для ховеров, второстепенных действий)
    /// </summary>
    public readonly static Color SecondaryAccent = ColorTranslator.FromHtml("#A79DFF");

    /// <summary>
    /// Цвет текста по умолчанию (основной контент)
    /// </summary>
    public readonly static Color TextPrimary = ColorTranslator.FromHtml("#2D2B3A");

    /// <summary>
    /// Цвет второстепенного текста (время, метаданные, подписи)
    /// </summary>
    public readonly static Color TextSecondary = ColorTranslator.FromHtml("#7A778F");

    /// <summary>
    /// Цвет текста на акцентном фоне (например, белый текст на кнопке)
    /// </summary>
    public readonly static Color TextOnAccent = ColorTranslator.FromHtml("#FFFFFF");

    /// <summary>
    /// Инвертированный цвет текста на акцентном фоне (например, чёрный текст на кнопке)
    /// </summary>
    public readonly static Color TextOnAccentInverted = ColorTranslator.FromHtml("#000000");

    /// <summary>
    /// Цвет разделителей и лёгких границ
    /// </summary>
    public readonly static Color DividerColor = ColorTranslator.FromHtml("#E4E6F0");

    /// <summary>
    /// Цвет фона поля ввода сообщения
    /// </summary>
    public readonly static Color InputFieldBackground = ColorTranslator.FromHtml("#F8F9FF");

    /// <summary>
    /// Цвет ошибки или предупреждения (например, недоставленное сообщение)
    /// </summary>
    public readonly static Color ErrorColor = ColorTranslator.FromHtml("#FF6B6B");

    /// <summary>
    /// Цвет запрета присоединения к комнате
    /// </summary>
    public readonly static Color ConnectionDisabled = Color.Gray;
}
