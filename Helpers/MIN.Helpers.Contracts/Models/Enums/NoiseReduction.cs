using System.ComponentModel;

namespace MIN.Helpers.Contracts.Models.Enums;

/// <summary>
/// Выбранное шумоподавления
/// </summary>
public enum NoiseReduction : int
{
    /// <summary>
    /// Ничего
    /// </summary>
    [Description("Ничего")]
    Nothing = 0,

    /// <summary>
    /// Шумоподавление Onnx
    /// </summary>
    [Description("Шумоподавление Onnx")]
    Onnx = 1,
}
