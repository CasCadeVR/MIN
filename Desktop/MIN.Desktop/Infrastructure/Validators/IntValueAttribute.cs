using System.ComponentModel.DataAnnotations;

namespace MIN.Desktop.Infrastructure.Validators;

/// <summary>
/// Проверяет, можно ли использовать значение в качестве числа
/// </summary>
public sealed class IntValueAttribute : ValidationAttribute
{
    /// <inheritdoc />
    protected override ValidationResult? IsValid(object? value, ValidationContext context)
        => value is int
            ? ValidationResult.Success
            : new ValidationResult(
            "Введённое значение должно быть числом");
}
