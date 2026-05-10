using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Messaging.Stateless.RoomRelated.RoomInfo;
using MIN.Desktop.Components;
using MIN.Desktop.Contracts.Interfaces;
using MIN.Desktop.Contracts.Schemes;
using MIN.Desktop.Views.Components;
using MIN.Desktop.Views.Forms.HelperForms;
using MIN.Desktop.Views.Panels.SidePanelViews;

namespace MIN.Desktop.Views.Panels.PanelViews.ChatPanel;

public partial class ChatPanelView
{
    #region Resizing

    private readonly System.Windows.Forms.Timer resizeTimer = new() { Interval = 150 };

    private bool isResizing;

    private void InitializeResizeTimer()
    {
        resizeTimer.Tick += (s, e) =>
        {
            resizeTimer.Stop();
            isResizing = false;
            PerformResize();
        };
    }

    private void PerformResize()
    {
        chatFlow.SuspendLayout();
        try
        {
            foreach (ChatMessageRow row in chatFlow.Controls)
            {
                row.Width = chatFlow.Width - row.Margin.Horizontal;
                var child = row.container.Controls[0];
                if (child is IResizableComponent resizableComponent)
                {
                    row.Height = resizableComponent.ResizeOutOfPrefferedSize() + row.Padding.Top;
                }
            }
        }
        finally
        {
            chatFlow.ResumeLayout();
        }
    }

    private void ResizeMessageTextBox()
    {
        var additionalRowsHeight = 0;
        for (var i = tableLayoutPanelButtons.RowCount - 2; i >= 0; i--)
        {
            additionalRowsHeight += Convert.ToInt32(tableLayoutPanelButtons.RowStyles[i].Height);
        }

        tableLayoutPanelButtons.Height = messageTextBox.UpdateHeight()
            + tableLayoutPanelButtons.Margin.Vertical + additionalRowsHeight;
    }

    private void chatFlow_Resize(object sender, EventArgs e)
    {
        if (Width <= MinimumSize.Width + splitContainerSideBar.Panel2.Width)
        {
            if (!splitContainerSideBar.Panel2Collapsed)
            {
                splitContainerSideBar.Panel2Collapsed = true;
            }
            aboutButton.Visible = false;
        }
        else
        {
            aboutButton.Visible = true;
        }

        if (!isResizing)
        {
            isResizing = true;
            resizeTimer.Stop();
            resizeTimer.Start();
        }
    }

    private void participantsFlow_Resize(object sender, EventArgs e)
    {
        foreach (ParticipantCard card in participantsFlow.Controls.OfType<ParticipantCard>())
        {
            card.Width = participantsFlow.Width - participantsFlow.Margin.Horizontal * 2;
        }
    }

    #endregion

    #region Button event attachment

    private async void sendButton_Click(object sender, EventArgs e) => await SendMessage();

    private async void editButton_Click(object sender, EventArgs e)
    {
        if (room == null)
        {
            return;
        }

        var editForm = new RoomCreateForm(new RoomInfo(room));
        var result = editForm.ShowDialog();

        if (result == DialogResult.Abort)
        {
            await DisposeAsync();
            navigationService.NavigateTo<DiscoveryPanelView>();
        }
        else if (result == DialogResult.OK
            && (editForm.Room.Name != room.Name
            || editForm.Room.MaximumParticipants != room.MaximumParticipants))
        {
            await featureCollection.Core.MessageRouter.RouteAsync(new RoomInfoUpdatedMessage
            {
                Room = editForm.Room
            }, roomId, localParticipant.Id, formCts.Token);
        }
    }

    private void fileButton_Click(object sender, EventArgs e)
    {
        uiContext.Post(_ =>
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                foreach (var filePath in openFileDialog.FileNames)
                {
                    UploadFile(filePath);
                }
            }
        }, this);
    }

    private async void disconnectButton_Click(object sender, EventArgs e)
    {
        await DisposeAsync();
        navigationService.NavigateTo<DiscoveryPanelView>();
    }

    private void closeButton_Click(object sender, EventArgs e)
    {
        splitContainerSideBar.Panel2Collapsed = true;
        chatFlow_Resize(sender, e);
    }

    private void aboutButton_Click(object sender, EventArgs e)
    {
        splitContainerSideBar.Panel2Collapsed = !splitContainerSideBar.Panel2Collapsed;
        chatFlow_Resize(sender, e);
    }

    #endregion

    #region MesssageTextBox events

    private void messageTextBox_TextChanged(object sender, EventArgs e) => ResizeMessageTextBox();

    private void messageTextBox_KeyPress(object sender, KeyPressEventArgs e)
    {
        if (e.KeyChar == '\r') // Enter
        {
            if ((ModifierKeys & Keys.Shift) == 0)
            {
                _ = SendMessage();
                ResizeMessageTextBox();
                e.Handled = true;
            }
        }
    }

    #endregion

    #region Drag

    private void chatFlow_DragEnter(object sender, DragEventArgs e)
    {
        e.Effect = DragDropEffects.Move;
        splitContainerSideBar.Panel1.Padding = new Padding(8);
        splitContainerSideBar.Panel1.BackColor = ColorScheme.ChatPanelFileDropBackground;
    }

    private void chatFlow_DragLeave(object sender, EventArgs e)
    {
        splitContainerSideBar.Panel1.Padding = new Padding(0);
    }

    private void chatFlow_DragDrop(object sender, DragEventArgs e)
    {
        if (e.Data == null || e.Data?.GetData(DataFormats.FileDrop) == null)
        {
            return;
        }

        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            var files = (string[])e.Data!.GetData(DataFormats.FileDrop)!;
            foreach (var filePath in files)
            {
                UploadFile(filePath);
            }
        }
        splitContainerSideBar.Panel1.Padding = new Padding(0);
    }

    #endregion
}
