using System;
using System.ComponentModel;
using Avalonia.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using MIN.Desktop.Contracts.Enums;
using MIN.Desktop.ViewModels.Base;

namespace MIN.Desktop.ViewModels.Modals;

/// <summary>
/// Модель окна диалога
/// </summary>
public partial class DialogBoxViewModel : ModalViewModelBase
{
    [ObservableProperty]
    public partial string? WindowTitle { get; set; }

    [ObservableProperty]
    public partial string Title { get; set; } = "";

    [ObservableProperty]
    public partial double TitleFontSize { get; set; } = 24;

    [ObservableProperty]
    public partial string Description { get; set; } = "";

    [ObservableProperty]
    public partial double DescriptionFontSize { get; set; } = 14;

    [ObservableProperty]
    public partial ButtonOptions ButtonOptions { get; set; } = ButtonOptions.Ok;

    /// <summary>
    /// Горячая клавиша на действие OK
    /// </summary>
    public KeyGesture OkHotkey { get; } = new(Key.Return);

    /// <summary>
    /// Горячая клавиша на действие NO
    /// </summary>
    public KeyGesture NoHotkey { get; } = new(Key.Escape);

    /// <inheritdoc />
    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(Title):
            case nameof(Description):
                if (WindowTitle is null or "")
                {
                    WindowTitle = string.IsNullOrEmpty(Title) ? WindowTitle : Title;
                }
                if (WindowTitle is null or "" && Description is not (null or ""))
                {
                    WindowTitle = $"{Description[..Math.Min(30, Description.Length)]}...";
                }
                break;
        }

        base.OnPropertyChanged(e);
    }
}
