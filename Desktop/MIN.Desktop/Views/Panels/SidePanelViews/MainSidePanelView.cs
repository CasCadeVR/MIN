using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Services.Rooms;
using MIN.Core.Stores.Contracts.Registries.Models;
using MIN.Core.Stores.Factories;
using MIN.Core.Stores.Services;
using MIN.Core.Transport.NamedPipes.Models;
using MIN.Desktop.Contracts.Views.PanelViews;
using MIN.Discovery.Services;
using MIN.Helpers.Contracts.Models;
using MIN.Helpers.Services;

namespace MIN.Desktop.Views.Panels.SidePanelViews;

/// <summary>
/// Главная панель
/// </summary>
public partial class MainSidePanelView : StyledPanelView
{


    /// <summary>
    /// Инициализирует новый экземпляр <see cref="MainSidePanelView"/>
    /// </summary>
    public MainSidePanelView()
    {
        InitializeComponent();
    }


    private async void createRoom_Click(object sender, EventArgs e)
    {
        var roomCreateForm = new RoomCreateForm();

        if (roomCreateForm.ShowDialog() != DialogResult.OK)
        {
            return;
        }

        var room = roomCreateForm.Room;

        if (!ResolveParticipant())
        {
            return;
        }

        localParticipant = identityService.SelfPartcipant.ToParticipantInfo();
        var context = roomFactory.GetOrCreateContext(room.Id);

        context.Connections.RegisterLocalParticipant(localParticipant);
        room.HostParticipant = localParticipant;

        try
        {
            roomStore.Add(room);

            var roomInfo = new RoomInfo(room);
            await roomHoster.StartHostingAsync(roomInfo, cts.Token);

            context.Participants.AddParticipant(localParticipant);

            await discoveryService.StartDiscoveryAsync(roomInfo.Id, cts.Token);

            OpenChatForm(roomStore.GetRoom(room.Id),
                CoreRegistryConstants.LocalConnectionId,
                isHost: true, new NamedPipeEndpoint()
                {
                    MachineName = Environment.MachineName,
                });
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Создание комнаты прошло не успешно: {ex.Message}",
                "Ошибка",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    private void settingsButton_Click(object sender, EventArgs e)
    {
        var settingsForm = new SettingsForm(Settings, version, logger);
        if (settingsForm.ShowDialog() == DialogResult.OK)
        {
            settingsProvider.SaveSettings(settingsForm.Settings);
        }
    }

    /// <inheritdoc />
    public override void ApplyStylings()
    {
        //discoveryProgressBar.ForeColor = ColorScheme.PrimaryAccent;
    }
}
