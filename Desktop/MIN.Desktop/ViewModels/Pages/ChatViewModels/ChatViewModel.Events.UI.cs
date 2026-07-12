using System;
using System.Threading.Tasks;
using System.Timers;
using Avalonia.Input;
using CommunityToolkit.Mvvm.Input;
using MIN.Core.Entities.Contracts.Enums;
using MIN.Desktop.ViewModels.Base;

namespace MIN.Desktop.ViewModels.Pages.ChatViewModels;

/// <summary>
/// Методы использования сервисов для чата
/// </summary>
public partial class ChatViewModel : RoutableViewModelBase
{
    private const string UriDataFormat = "UniformResourceLocator";

    private readonly Timer typingTimer = new() { Interval = 3000 };

    private bool isParentWindowActive = true;

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

    #region Resizing

    private void PerformResize()
    {
        //chatFlow.SuspendLayout();
        //try
        //{
        //    foreach (ChatMessageRow row in chatFlow.Controls)
        //    {
        //        row.Width = chatFlow.Width - row.Margin.Horizontal;
        //        var child = row.container.Controls[0];
        //        if (child is IResizableComponent resizableComponent)
        //        {
        //            row.Height = resizableComponent.ResizeOutOfPrefferedSize() + row.Padding.Top;
        //        }
        //    }
        //}
        //finally
        //{
        //    chatFlow.ResumeLayout();
        //}
    }

    private void chatFlow_Resize(object sender, EventArgs e)
    {
        //if (Width <= MinimumSize.Width + splitContainerSideBar.Panel2.Width)
        //{
        //    if (!splitContainerSideBar.Panel2Collapsed)
        //    {
        //        splitContainerSideBar.Panel2Collapsed = true;
        //    }
        //    aboutButton.Visible = false;
        //}
        //else
        //{
        //    aboutButton.Visible = true;
        //}

        //if (!isResizing)
        //{
        //    isResizing = true;
        //    resizeTimer.Stop();
        //    resizeTimer.Start();
        //}
    }

    private void participantsFlow_Resize(object sender, EventArgs e)
    {
        //foreach (ParticipantCard card in participantsFlow.Controls.OfType<ParticipantCard>())
        //{
        //    card.Width = participantsFlow.Width - participantsFlow.Margin.Horizontal * 2;
        //}
    }

    #endregion

    #region Context menu strips events

    private void InitializeContextMenuStrips()
    {
        //chatActionContextMenuStrip.UploadFileClick += () =>
        //{
        //    if (openFileDialog.ShowDialog() == DialogResult.OK)
        //    {
        //        foreach (var filePath in openFileDialog.FileNames)
        //        {
        //            UploadFile(filePath);
        //        }
        //    }
        //};

        //chatActionContextMenuStrip.StartSessionClick += () =>
        //{
        //    var downloadedSessions = featureCollection.Sessions.SessionScanner.DownloadedSessions.Values;
        //    if (!downloadedSessions.Any())
        //    {
        //        InAppNotifier.Info("У вас не установлена ни одна сессия!");
        //        return;
        //    }

        //    var choosingForm = new SessionChoosingForm(downloadedSessions);
        //    choosingForm.OnSelected += (session) =>
        //    {
        //        choosingForm.Close();
        //        SendSessionStartMessage(session);
        //    };
        //    choosingForm.ShowDialog();
        //};
    }

    #endregion

    #region Button event attachment

    [RelayCommand]
    private async Task EditRoom()
    {
        //if (room == null)
        //{
        //    return;
        //}

        //var editForm = new RoomCreateForm(new RoomInfo(room));
        //var result = editForm.ShowDialog();

        //if (result == DialogResult.Abort)
        //{
        //    await DisposeAsync();
        //    ChangeView(discoveryViewModel);
        //}
        //else if (result == DialogResult.OK
        //    && (editForm.Room.Name != room.Name
        //    || editForm.Room.MaximumParticipants != room.MaximumParticipants))
        //{
        //    try
        //    {
        //        await featureCollection.Chat.ChatRoomService.SendUpdatedRoomInfoAsync(editForm.Room, formCts.Token);
        //    }
        //    catch (Exception ex)
        //    {
        //        InAppNotifier.Error(ex.Message);
        //    }
        //}
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

    private void messageTextBox_KeyPress(object sender, EventArgs e)
    {
        //if (e.KeyChar == '\r') // Enter
        //{
        //    if ((ModifierKeys & Keys.Shift) == 0)
        //    {
        //        _ = SendMessage();
        //        ResizeMessageTextBox();
        //        e.Handled = true;
        //    }
        //}
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

    private void messageTextBox_KeyDown(object sender, KeyEventArgs e)
    {
        //if (e.Control && e.KeyCode == Keys.V)
        //{
        //    if (Clipboard.ContainsFileDropList())
        //    {
        //        foreach (var filePath in Clipboard.GetFileDropList())
        //        {
        //            if (filePath != null)
        //            {
        //                UploadFile(filePath);
        //            }
        //        }
        //        e.Handled = true;
        //        e.SuppressKeyPress = true;
        //    }
        //    else if (Clipboard.ContainsImage())
        //    {
        //        var image = Clipboard.GetImage();
        //        if (image == null)
        //        {
        //            e.Handled = true;
        //            e.SuppressKeyPress = true;
        //            return;
        //        }

        //        var timestamp = DateTime.Now.ToString("yyyy-dd-MM-HH-mm-ss-fffff");
        //        var tempPath = Path.Combine(Path.GetTempPath(), $"clipboard_{timestamp}.png");
        //        image.Save(tempPath, ImageFormat.Png);
        //        UploadFile(tempPath);
        //        e.Handled = true;
        //        e.SuppressKeyPress = true;
        //    }
        //}
    }

    #endregion

    #region Drag

    //private void chatFlow_DragEnter(object sender, DragEventArgs e)
    //{
    //    e.Effect = DragDropEffects.Copy;
    //    splitContainerSideBar.Panel1.Padding = new Padding(8);
    //    splitContainerSideBar.Panel1.BackColor = ColorScheme.ChatPanelFileDropBackground;
    //}

    //private void chatFlow_DragLeave(object sender, EventArgs e)
    //{
    //    splitContainerSideBar.Panel1.Padding = new Padding(0);
    //}

    //private void chatFlow_DragOver(object sender, DragEventArgs e)
    //{
    //    e.Effect = DragDropEffects.Copy;
    //}

    //private void chatFlow_DragDrop(object sender, DragEventArgs e)
    //{
    //    if (e.Data == null)
    //    {
    //        return;
    //    }

    //    if (e.Data.GetDataPresent(DataFormats.FileDrop))
    //    {
    //        var files = (string[])e.Data.GetData(DataFormats.FileDrop)!;
    //        foreach (var filePath in files)
    //        {
    //            UploadFile(filePath);
    //        }
    //    }
    //    else if (e.Data.GetDataPresent(UriDataFormat))
    //    {
    //        var url = (string)e.Data.GetData(UriDataFormat)!;
    //        if (url.StartsWith("file://"))
    //        {
    //            UploadFile(new Uri(url).LocalPath);
    //        }
    //    }

    //    splitContainerSideBar.Panel1.Padding = new Padding(0);
    //}

    #endregion
}
