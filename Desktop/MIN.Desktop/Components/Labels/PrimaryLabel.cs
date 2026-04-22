using MIN.Desktop.Contracts;
using MIN.Desktop.Contracts.Schemes;

namespace MIN.Desktop.Components.Labels;

/// <summary>
/// Кастомный <see cref="BaseLabel"/> для обычного текста
/// </summary>
public class PrimaryLabel : BaseLabel
{
    /// <summary>
    /// Инициализирует новый экземпляр <see cref="PrimaryLabel"/>
    /// </summary>
    public PrimaryLabel()
    {
        ForeColor = ColorScheme.TextPrimary;
        Font = FontScheme.Default;
    }
}
