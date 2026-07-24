using System;

namespace MIN.Desktop.Contracts.Enums;

/// <summary>
/// Тип расположения страницы на экране
/// </summary>
[Flags]
public enum ButtonOptions
{
    /// <summary>
    /// Ок
    /// </summary>
    Ok = 1 << 0,

    /// <summary>
    /// Да
    /// </summary>
    Yes = 1 << 1,

    /// <summary>
    /// Нет
    /// </summary>
    No = 1 << 2,

    /// <summary>
    /// Да и нет
    /// </summary>
    YesNo = Yes | No,
}
