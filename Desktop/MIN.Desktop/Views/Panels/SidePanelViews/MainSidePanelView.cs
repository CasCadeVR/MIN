using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Stores.Contracts.Registries.Models;
using MIN.Core.Transport.Contracts.Interfaces;
using MIN.Core.Transport.NamedPipes.Models;
using MIN.Desktop.Contracts.Enums;
using MIN.Desktop.Contracts.Interfaces;
using MIN.Desktop.Contracts.Views.PanelViews;
using MIN.Desktop.Views.Panels.PanelViews;
using MIN.DI;
using MIN.Helpers.Contracts.Extensions;

namespace MIN.Desktop.Views.Panels.SidePanelViews;

/// <summary>
/// Главная боковая панель
/// </summary>
public partial class MainSidePanelView : StyledPanelView
{
    private readonly IMinFeatureCollection featureCollection;
    private readonly INavigationService navigationService;

    /// <inheritdoc />
    public override PanelType PanelType => PanelType.Side;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="MainSidePanelView"/>
    /// </summary>
    public MainSidePanelView(IMinFeatureCollection featureCollection,
        INavigationService navigationService)
    {
        InitializeComponent();
        this.featureCollection = featureCollection;
        this.navigationService = navigationService;
    }

    private bool ResolveParticipant()
    {
        var settings = featureCollection.Helper.SettingsProvider.GetSettings();
        var localParticipant = featureCollection.Helper.IdentityService.SelfPartcipant.ToParticipantInfo();

        if (settings.DefaultParticipantName != string.Empty)
        {
            localParticipant.Name = settings.DefaultParticipantName;
            featureCollection.Helper.IdentityService.SetParticipant(localParticipant);
        }
        else
        {
            var participantCreateForm = new ParticipantCreateForm(featureCollection.Helper.IdentityService);
            if (participantCreateForm.ShowDialog() != DialogResult.OK)
            {
                return false;
            }
            settings.DefaultParticipantName = featureCollection.Helper.IdentityService.SelfPartcipant.Name;
            featureCollection.Helper.SettingsProvider.SaveSettings(settings);
        }
        return true;
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

        var localParticipant = featureCollection.Helper.IdentityService.SelfPartcipant.ToParticipantInfo();
        var context = featureCollection.Core.RoomFactory.GetOrCreateContext(room.Id);

        context.Connections.RegisterLocalParticipant(localParticipant);
        room.HostParticipant = localParticipant;

        try
        {
            featureCollection.Core.RoomStore.Add(room);

            var roomInfo = new RoomInfo(room);
            await featureCollection.Core.RoomHoster.StartHostingAsync(roomInfo, cts.Token);

            context.Participants.AddParticipant(localParticipant);

            await featureCollection.Discovery.DiscoveryService.StartDiscoveryAsync(roomInfo.Id, cts.Token);

            navigationService.NavigateTo<ChatPanelView, (Guid roomId, Guid connectionId, IEndpoint endpoint)>(
                (room.Id,
                CoreRegistryConstants.LocalConnectionId,
                new NamedPipeEndpoint()
                {
                    MachineName = Environment.MachineName,
                }));
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
        navigationService.NavigateTo<SettingsSidePanelView>();
    }

    private void discoveryButton_Click(object sender, EventArgs e)
    {
        navigationService.NavigateTo<DiscoveryPanelView>();
    }
}
