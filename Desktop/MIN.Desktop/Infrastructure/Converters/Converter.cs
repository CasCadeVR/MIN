using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;

namespace MIN.Desktop.Infrastructure.Converters;

/// <summary>
/// Базовый конвертор, используемый вместо выражений в Bindingах
/// </summary>
public abstract class Converter<TSelf> : MarkupExtension, IValueConverter
    where TSelf : Converter<TSelf>, new()
{
    private static TSelf Instance { get; } = new();

    /// <summary>
    /// Предоставить экземпляр из DI
    /// </summary>
    public sealed override object ProvideValue(IServiceProvider serviceProvider) => Instance;

    /// <inheritdoc cref="IValueConverter.Convert(object?, Type, object?, CultureInfo)"/>
    public abstract object Convert(object? value, Type targetType, object? parameter, CultureInfo culture);

    /// <inheritdoc cref="IValueConverter.ConvertBack(object?, Type, object?, CultureInfo)"/>
    public virtual object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotSupportedException();
}
