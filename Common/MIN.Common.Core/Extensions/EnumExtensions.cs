using System.ComponentModel;
using System.Reflection;

namespace MIN.Common.Core.Extensions;

/// <summary>
/// Расширения для <see cref="Enum"/>
/// </summary>
public static class EnumExtensions
{
    /// <summary>
    /// Получить описание в виде текста от причины отключения
    /// </summary>
    public static string GetDescription(this Enum value)
    {
        var field = value.GetType().GetField(value.ToString());
        var attribute = (DescriptionAttribute?)field?.GetCustomAttribute(typeof(DescriptionAttribute));
        return attribute != null ? attribute.Description : value.ToString();
    }
}
