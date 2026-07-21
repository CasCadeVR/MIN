using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Xaml.Interactivity;

namespace MIN.Desktop.Infrastructure.Behaviors;

/// <summary>
/// <see cref="Control"/>, позволяющий принимать файлы через drag and drop
/// </summary>
public class DropFilesBehavior : StyledElementBehavior<Control>
{
    /// <summary>
    /// Команда установки файлов
    /// </summary>
    public readonly static AttachedProperty<ICommand> DropFilesCommandProperty =
        AvaloniaProperty.RegisterAttached<DropFilesBehavior, Control, ICommand>("DropFilesCommand");

    static DropFilesBehavior()
    {
        DropFilesCommandProperty.Changed.AddClassHandler<Control>(OnChanged);
    }

    /// <summary>
    /// Установить команду drop файлов
    /// </summary>
    public static void SetDropFilesCommand(Control element, ICommand value) =>
        element.SetValue(DropFilesCommandProperty, value);

    /// <summary>
    /// Получить команду drop файлов
    /// </summary>
    public static ICommand GetDropFilesCommand(Control element) =>
        element.GetValue(DropFilesCommandProperty);

    private static void OnChanged(Control control, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.NewValue is ICommand)
        {
            control.AddHandler(DragDrop.DropEvent, OnDrop);
        }
        else
        {
            control.RemoveHandler(DragDrop.DropEvent, OnDrop);
        }
    }

    private static void OnDrop(object? sender, DragEventArgs e)
    {
        if (sender is not Control control)
        {
            return;
        }

        var cmd = GetDropFilesCommand(control);
        if (cmd?.CanExecute(null) != true)
        {
            return;
        }

        var files = new List<string>();

        if (e.DataTransfer.Contains(DataFormat.File))
        {
            files.AddRange(e.DataTransfer.TryGetFiles()!.Select(f => f.Path.LocalPath));
        }

        if (e.DataTransfer.Contains(DataFormat.Text))
        {
            var text = e.DataTransfer.TryGetText();
            if (Uri.TryCreate(text, UriKind.Absolute, out var uri) && uri.IsFile)
            {
                files.Add(uri.LocalPath);
            }
        }

        if (files.Count > 0)
        {
            cmd.Execute(files);
        }
    }
}
