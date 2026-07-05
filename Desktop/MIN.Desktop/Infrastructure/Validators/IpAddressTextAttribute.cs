using System;
using System.ComponentModel.DataAnnotations;
using MIN.Desktop.Infrastructure.Services;

namespace MIN.Desktop.Infrastructure.Validators;

/// <summary>
/// Проверяет, можно ли использовать значение в качестве IP адреса
/// </summary>
public sealed class IpAddressTextAttribute : TypedValidationAttribute<string>
{
    /// <inheritdoc />
    protected override ValidationResult? IsValid(string? value, ValidationContext context)
    {
        try
        {
            IpAddressParser.ValidateIP(value ?? string.Empty);
            return ValidationResult.Success;
        }
        catch (Exception ex) when (ex is InvalidOperationException)
        {
            return new ValidationResult($"{context.DisplayName} введён в неправильном формате");
        }
    }
}
