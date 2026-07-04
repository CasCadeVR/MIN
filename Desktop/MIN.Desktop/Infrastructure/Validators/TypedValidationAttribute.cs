using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace MIN.Desktop.Infrastructure.Validators;

/// <summary>
/// Аттрибут валидации типа
/// </summary>
public abstract class TypedValidationAttribute<T> : ValidationAttribute
{
    /// <inheritdoc />
    protected abstract ValidationResult? IsValid(T? value, ValidationContext context);

    /// <inheritdoc />
    protected override ValidationResult? IsValid(object? value, ValidationContext context)
    {
        if (value == null)
        {
            return IsValid(default, context);
        }
        if (value is not T typedValue)
        {
            return new ValidationResult($"The field {context.DisplayName} must be of type {typeof(T).Name}.");
        }
        return IsValid(typedValue, context);
    }

    /// <inheritdoc />
    static protected TResult ReadProperty<TResult>(ValidationContext context, string propertyName)
    {
        if (string.IsNullOrWhiteSpace(propertyName))
        {
            return default!;
        }
        var value = context.ObjectType.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic)?.GetValue(context.ObjectInstance);
        return value is TResult tValue ? tValue : default!;
    }
}
