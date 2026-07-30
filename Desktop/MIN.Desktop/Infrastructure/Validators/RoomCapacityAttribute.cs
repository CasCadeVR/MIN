using System.ComponentModel.DataAnnotations;
using MIN.Desktop.Contracts.Constants;

namespace MIN.Desktop.Infrastructure.Validators;

/// <summary>
/// Проверяет, можно ли использовать значение в качестве размера комнаты
/// </summary>
public sealed class RoomCapacityAttribute : ValidationAttribute
{
    /// <inheritdoc />
    protected override ValidationResult? IsValid(object? value, ValidationContext context)
        => value is int v && v >= 1 && v <= DesktopConstants.MaximumParticipantsInRoom
            ? ValidationResult.Success
            : new ValidationResult(
            $"Количество участников должно быть от 1 до {DesktopConstants.MaximumParticipantsInRoom}");
}
