using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Input.Platform;
using Avalonia.Labs.Gif;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MIN.Desktop.Infrastructure.Services;
using MIN.Desktop.ViewModels.Base;
using MIN.Sessions.Core.Services.Contracts.Models;

namespace MIN.Desktop.ViewModels.Cards;

/// <summary>
/// View модель сессии
/// </summary>
public partial class DownloadedSessionViewModel : CardViewModelBase, IDisposable
{
    private readonly IClipboard? clipboard;

    private FileStream? gifStream;
    private GifStreamSource gifSessionImage = null!;

    /// <summary>
    /// Сессия
    /// </summary>
    [ObservableProperty]
    public partial Session Session { get; set; }

    /// <summary>
    /// Версия сессии
    /// </summary>
    [ObservableProperty]
    public partial string Version { get; set; } = string.Empty;

    /// <summary>
    /// Инфа о максимальном кол-ве участников
    /// </summary>
    [ObservableProperty]
    public partial string MaximumParticipantInfo { get; set; } = string.Empty;

    /// <summary>
    /// Ссылка на установку сессии
    /// </summary>
    [ObservableProperty]
    public partial string DownloadLink { get; set; } = string.Empty;

    /// <summary>
    /// Выбрана ли карточка
    /// </summary>
    [ObservableProperty]
    public partial bool IsSelected { get; set; }

    /// <summary>
    /// Изображение сессии
    /// </summary>
    public Bitmap? SessionImage { get; set; }

    /// <summary>
    /// .gif Изображение сессии
    /// </summary>
    public GifStreamSource GifSessionImage
    {
        get => gifSessionImage;
        private set
        {
            gifSessionImage = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Событие по нажатию на карточку
    /// </summary>
    public Action<bool>? OnClicked { get; set; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="DownloadedSessionViewModel"/>
    /// </summary>
    public DownloadedSessionViewModel(Session session, IClipboard? clipboard)
    {
        Session = session;
        this.clipboard = clipboard;

        FillLabels();
    }

    private void FillLabels()
    {
        DownloadLink = Session.DownloadLink;
        Version = $"Версия: {Session.Version}";
        MaximumParticipantInfo = "Максимум участников: " + (Session.MaximumParticipants.HasValue
            ? Session.MaximumParticipants.Value
            : "Неограничено");

        if (Session.ThumbnailFileName != null)
        {
            var bytes = File.ReadAllBytes(Session.GetThumbnailPath());
            using var ms = new MemoryStream(bytes);

            var format = Session.ThumbnailFileName.TakeLast(3).ToString();

            gifStream?.Dispose();
            gifStream = null;

            if (format == "gif")
            {
                GifSessionImage = GifStreamSource.FromStream(ms);
            }
            else if (format == "svg")
            {
                SessionImage = ImageHelper.SvgToBitmap(ms);
            }
            else
            {
                SessionImage = new Bitmap(ms);
            }
        }
    }

    /// <summary>
    /// Отменить выбор карточки
    /// </summary>
    public void Unselect()
    {
        IsSelected = false;
    }

    /// <summary>
    /// Выбрать карточку
    /// </summary>
    [RelayCommand]
    public void Select()
    {
        IsSelected = !IsSelected;
        OnClicked?.Invoke(IsSelected);
    }

    [RelayCommand]
    private async Task CopyLink()
    {
        if (clipboard == null)
        {
            return;
        }

        try
        {
            await clipboard.SetTextAsync(Session.DownloadLink);
            DownloadLink = "Скопировано!";
        }
        catch { }
    }

    /// <inheritdoc cref="IDisposable.Dispose"/>
    public override void Dispose()
    {
        gifStream?.Dispose();
        gifStream = null;

        base.Dispose();
    }
}
