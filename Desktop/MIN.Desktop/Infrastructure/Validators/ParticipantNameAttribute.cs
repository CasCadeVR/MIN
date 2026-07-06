using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;

namespace MIN.Desktop.Infrastructure.Validators;

/// <summary>
/// Проверяет, можно ли использовать значение в качестве имени участника
/// </summary>
public sealed class ParticipantNameAttribute : TypedValidationAttribute<string>
{
    private const int MAX_PARTICIPANT_NAME = 70;

    readonly static internal char[] InvalidPathCharacters = Path.GetInvalidFileNameChars();

    /// <inheritdoc />
    protected override ValidationResult? IsValid(string? value, ValidationContext context)
    {
        if (value == null)
        {
            return ValidationResult.Success;
        }
        if (value.Length > MAX_PARTICIPANT_NAME)
        {
            return new ValidationResult($"{context.DisplayName} не может быть больше {MAX_PARTICIPANT_NAME} символов");
        }
        var indexOfAny = value.IndexOfAny(InvalidPathCharacters);
        if (indexOfAny > -1)
        {
            return new ValidationResult($"{context.DisplayName} не может содержать '{value[indexOfAny]}'. Вот запрещённые символы: {string.Join(' ', InvalidPathCharacters.Where(c => c > 31))}");
        }

        return ValidationResult.Success;
    }
}
