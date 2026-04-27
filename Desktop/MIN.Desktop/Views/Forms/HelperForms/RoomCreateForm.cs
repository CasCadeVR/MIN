using MIN.Core.Entities.Contracts.Models;
using MIN.Desktop.Contracts.Schemes;
using MIN.Desktop.Contracts.Views.Forms;

namespace MIN.Desktop;

/// <summary>
/// Форма создания комнаты
/// </summary>
public partial class RoomCreateForm : StyledForm
{
    private readonly bool isNew;

    /// <summary>
    /// Редактируемая комната
    /// </summary>
    public RoomInfo Room { get; set; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="RoomCreateForm"/>
    /// </summary>
    public RoomCreateForm(RoomInfo? room = null)
    {
        isNew = room == null;
        Room = new RoomInfo();

        if (!isNew)
        {
            Room = new RoomInfo(room!);
        }

        InitializeComponent();

        Shown += (_, _) => roomName.Focus();
        var title = isNew ? "Создание комнаты" : "Редактирование комнаты";
        Title.Text = title;
        Text = "MIN - " + title;
    }

    /// <inheritdoc />
    protected override void ApplyStylings()
    {
        splitContainer.Panel1.BackColor = ColorScheme.PrimaryAccent;
        splitContainer.Panel2.BackColor = ColorScheme.MainPanelBackground;
        Title.ForeColor = ColorScheme.TextOnAccent;

        if (!isNew)
        {
            cancelButton.FlatAppearance.BorderColor = ColorScheme.ErrorColor;
            cancelButton.ForeColor = ColorScheme.ErrorColor;
            cancelButton.Text = "Удалить комнату";

            createButton.Text = "Сохранить";
            roomName.Text = Room.Name;
            roomMaximumCount.Value = Room.MaximumParticipants;
        }
    }

    private void ValidateRoom()
    {
        if (string.IsNullOrEmpty(roomName.Text))
        {
            throw new InvalidOperationException("Имя комнаты не может быть пустым");
        }

        if (!isNew && roomMaximumCount.Value < Room.ParticipantCount)
        {
            throw new InvalidOperationException("Максимальное количество участников не может быть меньше текущего количества участников");
        }
    }

    private void createButton_Click(object sender, EventArgs e)
    {
        try
        {
            ValidateRoom();
        }
        catch (Exception ex) when (ex is InvalidOperationException)
        {
            MessageBox.Show(
                ex.Message,
                "Ошибка валидации",
                MessageBoxButtons.OK,
                MessageBoxIcon.Exclamation
            );
            return;
        }

        Room.Name = roomName.Text;
        Room.MaximumParticipants = Convert.ToInt32(roomMaximumCount.Value);

        DialogResult = DialogResult.OK;
    }

    private void cancelButton_Click(object sender, EventArgs e)
    {
        if (isNew)
        {
            DialogResult = DialogResult.Cancel;
        }
        else
        {
            if (MessageBox.Show("Вы точно хотите удалить эту комнату?", "Удаление комнаты", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                DialogResult = DialogResult.Abort;
            }
        }
    }
}
