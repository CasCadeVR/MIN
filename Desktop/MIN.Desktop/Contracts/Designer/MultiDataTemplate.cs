using System;
using System.Collections.Generic;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Markup.Xaml.Templates;
using Avalonia.Metadata;

namespace MIN.Desktop.Contracts.Designer;

/// <summary>
/// Selects a <see cref="DataTemplate" /> based on its <see cref="DataTemplate.DataType" />.
/// </summary>
public class MultiDataTemplate : AvaloniaList<DataTemplate>, IRecyclingDataTemplate
{
    private readonly Dictionary<Type, Control> typeToControlCache = [];

    /// <summary>
    /// Содержимое
    /// </summary>
    [Content]
    public List<DataTemplate> Content { get; set; } = [];

    /// <summary>
    /// If true, caches the control objects generated from the data templates.
    /// </summary>
    public bool UseCache { get; set; } = true;

    /// <inheritdoc />
    public bool Match(object? data) => GetTemplateForType(data?.GetType()) != null;

    /// <inheritdoc />
    public Control? Build(object? data, Control? existing)
    {
        var type = data?.GetType();
        if (UseCache && type != null && typeToControlCache.TryGetValue(type, out var control))
        {
            return control;
        }
        var build = GetTemplateForType(type)?.Build(data);
        if (type != null && build != null)
        {
            typeToControlCache[type] = build;
        }

        return build ?? existing;
    }

    /// <inheritdoc />
    public Control Build(object? data) => GetTemplateForType(data?.GetType())?.Build(data) ?? new TextBlock { Text = "" };

    private DataTemplate? GetTemplateForType(Type? type)
    {
        if (type == null)
        {
            return null;
        }
        foreach (DataTemplate template in Content)
        {
            if (template.DataType?.IsAssignableTo(type) ?? false)
            {
                return template;
            }
        }
        return null;
    }
}
