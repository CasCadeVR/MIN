using System;
using System.Globalization;

namespace MIN.Desktop.Infrastructure.Converters;

/// <summary>
/// Конвертор для числовых сравнения значений
/// </summary>
public class NumericComparisonConverter : Converter<NumericComparisonConverter>
{
    /// <inheritdoc />
    public override object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not IComparable comparable || parameter is not string paramStr)
        {
            return false;
        }

        var op = "==";
        var operandStart = 0;

        if (paramStr.Length > 0 && paramStr[0] is '>' or '<')
        {
            if (paramStr.Length > 1 && paramStr[1] == '=')
            {
                op = paramStr[..2];
                operandStart = 2;
            }
            else
            {
                op = paramStr[..1];
                operandStart = 1;
            }
        }

        var operandStr = paramStr[operandStart..];
        if (!double.TryParse(operandStr, out var operand))
        {
            return false;
        }

        var comparableOperand = System.Convert.ChangeType(operand, comparable.GetType());
        var compareResult = comparable.CompareTo(comparableOperand);

        return op switch
        {
            ">" => compareResult > 0,
            ">=" => compareResult >= 0,
            "<" => compareResult < 0,
            "<=" => compareResult <= 0,
            _ => compareResult == 0
        };
    }
}
