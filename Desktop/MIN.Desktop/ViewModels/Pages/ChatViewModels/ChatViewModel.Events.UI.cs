using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MIN.Core.Entities.Contracts.Enums;
using MIN.Core.Entities.Contracts.Models;
using MIN.Desktop.Contracts.Enums;
using MIN.Desktop.Contracts.Models.ReferenceCommands.Layout;
using MIN.Desktop.Infrastructure.Extensions;
using MIN.Desktop.Infrastructure.Services;
using MIN.Desktop.ViewModels.Base;
using MIN.Desktop.ViewModels.Modals;
using MIN.Desktop.Views;

namespace MIN.Desktop.ViewModels.Pages.ChatViewModels;

/// <summary>
/// Методы использования сервисов для чата
/// </summary>
public partial class ChatViewModel : RoutableViewModelBase
{
    private readonly Timer typingTimer = new() { Interval = 3000 };

    private bool isParentWindowActive = true;

    [ObservableProperty]
    public partial int CaretIndex { get; set; }

    #region Layouting

    [ObservableProperty]
    public partial WindowLayout CurrentLayout { get; private set; }

    private void InitializeLayoutStyles()
    {
        if (parentWindow is MainWindow mainWindow)
        {
            CurrentLayout = mainWindow.CurrentLayout;
        }

        this.RegisterMessageListener<LayoutModeChangedReferenceCommand, ChatViewModel>((msg, _) =>
            CurrentLayout = msg.Layout);
    }

    /// <summary>
    /// Открыть боковую панель
    /// </summary>
    [RelayCommand]
    public void ToggleRightSideBar()
    {
        if (!chatSideBarViewModel.IsOpened)
        {
            ChangeView(chatSideBarViewModel);
        }
        else
        {
            chatSideBarViewModel.CloseView(this);
        }
    }

    [RelayCommand]
    private void ShowLeftSideBar()
    {
        ChangeView(mainSideBarViewModel);
    }

    #endregion

    #region Timers

    private void InitializeTypingTimer()
    {
        typingTimer.Elapsed += (s, e) => OnTypingTimerStop();
    }

    private void OnTypingTimerStop()
    {
        typingTimer.Stop();
        _ = SendSelfStatusChangedMessage(GetRestingStatus());
    }

    private OnlineStatus GetRestingStatus() => isParentWindowActive
            ? OnlineStatus.Online
            : OnlineStatus.Offline;

    #endregion

    #region Chat action events

    [RelayCommand]
    private async Task UploadFileClick()
    {
        var files = await parentWindow.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Выберите файл",
            AllowMultiple = true,
            FileTypeFilter =
            [
                new FilePickerFileType("Все файлы") { Patterns = ["*.*"] },
                new FilePickerFileType("Изображения") { Patterns = ["*.png", "*.jpg", "*.jpeg"] },
            ]
        });

        foreach (var file in files)
        {
            UploadFile(file.Path.LocalPath);
        }
    }

    [RelayCommand]
    private async Task StartSessionClick()
    {
        var downloadedSessions = featureCollection.Sessions.SessionScanner.DownloadedSessions.Values;
        if (!downloadedSessions.Any())
        {
            InAppNotifier.Info("У вас не установлена ни одна сессия!");
            return;
        }

        var choosingForm = await dialogService.ShowDialogAsync<SessionChoosingViewModel>();
        if (choosingForm! == true)
        {
            SendSessionStartMessage(choosingForm!.SelectedSession!);
        }
    }

    #endregion

    #region Button event attachment

    [RelayCommand]
    private async Task EditRoom()
    {
        var editForm = await dialogService.ShowDialogAsync<CreateRoomViewModel>(vm =>
        {
            vm.InitializeWithRoom(new RoomInfo(room));
        });

        if ((editForm! == true && editForm != null)
            && (editForm.Room.Name != room.Name
            || editForm.Room.MaximumParticipants != room.MaximumParticipants))
        {
            try
            {
                await featureCollection.Chat.ChatRoomService.SendUpdatedRoomInfoAsync(editForm.Room, formCts.Token);
            }
            catch (Exception ex)
            {
                InAppNotifier.Error(ex.Message);
            }
        }
    }

    #endregion

    #region MesssageTextBox events

    private void messageTextBox_TextChanged(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(SendingMessage))
        {
            OnTypingTimerStop();
        }
        else
        {
            if (!typingTimer.Enabled)
            {
                _ = SendSelfStatusChangedMessage(OnlineStatus.Typing);
            }

            typingTimer.Stop();
            typingTimer.Start();
        }
    }

    #endregion

    #region Parent form events

    private void InitializeParentFormWindowStateEvents()
    {
        parentWindow.Activated += Parent_Activated;
        parentWindow.Deactivated += Parent_Deactivate;
    }

    private void ClearParentFormEvents()
    {
        parentWindow.Activated -= Parent_Activated;
        parentWindow.Deactivated -= Parent_Deactivate;
    }

    private async void Parent_Deactivate(object? sender, EventArgs e)
    {
        typingTimer.Stop();
        await SendSelfStatusChangedMessage(OnlineStatus.Offline);
        isParentWindowActive = false;
    }

    private async void Parent_Activated(object? sender, EventArgs e)
    {
        await SendSelfStatusChangedMessage(OnlineStatus.Online);
        isParentWindowActive = true;
    }

    [RelayCommand]
    private async Task PasteData() => await PasteDataFromClipboard(false);

    [RelayCommand]
    private async Task PasteDataWithText() => await PasteDataFromClipboard(true);

    private async Task PasteDataFromClipboard(bool includingText = false)
    {
        var clipboard = parentWindow.Clipboard;

        if (clipboard == null)
        {
            return;
        }

        var formats = await clipboard.GetDataFormatsAsync();

        if (formats.Contains(DataFormat.File))
        {
            if (await clipboard.TryGetFilesAsync() is IEnumerable<IStorageItem> files)
            {
                foreach (var file in files)
                {
                    if (!string.IsNullOrEmpty(file.Path.AbsolutePath))
                    {
                        UploadFile(file.Path.AbsolutePath);
                    }
                }
                return;
            }
        }

        if (includingText)
        {
            if (formats.Contains(DataFormat.Text))
            {
                if (await clipboard.TryGetTextAsync() is string text)
                {
                    SendingMessage = SendingMessage.Insert(CaretIndex, text);
                    CaretIndex += text.Length;
                    return;
                }
            }
        }

        var image = await clipboard.TryGetBitmapAsync();
        if (image is Bitmap bitmap)
        {
            var timestamp = DateTime.Now.ToString("yyyy-dd-MM-HH-mm-ss-fffff");
            var tempPath = Path.Combine(Path.GetTempPath(), $"clipboard_{timestamp}.png");
            bitmap.Save(tempPath); // Avalonia 12: Bitmap.Save(string)
            UploadFile(tempPath);
        }
    }

    #endregion

    #region Drag

    [RelayCommand]
    private void DropFiles(List<string> paths)
    {
        foreach (var path in paths)
        {
            UploadFile(path);
        }
    }

    #endregion
}
