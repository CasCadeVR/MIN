using System;
using System.ComponentModel.DataAnnotations;

namespace MIN.Desktop.Infrastructure.Validators;

/// <summary>
/// Проверяет, не заканчивается ли значение указанным текстом
/// </summary>
public sealed class NotEndsWithAttribute(string text, StringComparison comparison = StringComparison.OrdinalIgnoreCase) : TypedValidationAttribute<string>
{
    /// <inheritdoc />
    protected override ValidationResult? IsValid(string? value, ValidationContext context)
    {
        if (value == null)
        {
            return ValidationResult.Success;
        }
        return value.EndsWith(text, comparison) ? new ValidationResult($"{context.DisplayName} must not contain the text '{text}' at the end.") : ValidationResult.Success;
    }
}
