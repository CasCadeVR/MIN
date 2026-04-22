using MIN.Desktop.Contracts;
using MIN.Desktop.Contracts.Schemes;

namespace MIN.Desktop.Components.Labels;

/// <summary>
/// Кастомный <see cref="BaseLabel"/> для текста заголовков 1 уровня
/// </summary>
public class Heading3Label : BaseLabel
{
    /// <summary>
    /// Инициализирует новый экземпляр <see cref="Heading1Label"/>
    /// </summary>
    public Heading3Label()
    {
        ForeColor = ColorScheme.TextPrimary;
        Font = FontScheme.Heading3;
    }
}
