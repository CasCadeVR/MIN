using MIN.Desktop.Contracts;
using MIN.Desktop.Contracts.Schemes;

namespace MIN.Desktop.Components.Labels;

/// <summary>
/// Кастомный <see cref="BaseLabel"/> для текста заголовков 1 уровня
/// </summary>
public class Heading1Label : BaseLabel
{
    /// <summary>
    /// Инициализирует новый экземпляр <see cref="Heading1Label"/>
    /// </summary>
    public Heading1Label()
    {
        ForeColor = ColorScheme.FormBackground;
        Font = FontScheme.Heading1;
    }
}
