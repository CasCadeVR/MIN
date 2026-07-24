using MIN.Desktop.Contracts.Interfaces;
using MIN.Desktop.Contracts.Schemes;

namespace MIN.Desktop.Views.Components.ChatMessages;

/// <summary>
/// Базовая карточка сообщения
/// </summary>
public partial class BaseChatMessageCard : UserControl, IStyled
{
    /// <summary>
    /// <inheritdoc cref="Panel"/>
    /// </summary>
    public Panel ContentPanel = new()
    {
        Padding = new Padding(0),
        Margin = new Padding(0),
    };

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="BaseChatMessageCard"/>
    /// </summary>
    public BaseChatMessageCard()
    {
        InitializeComponent();
        TableLayoutPanel.Controls.Add(ContentPanel, 0, 1);
        ContentPanel.Dock = DockStyle.Fill;
    }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="BaseChatMessageCard"/>
    /// </summary>
    public BaseChatMessageCard(string name,
       DateTime time,
       bool isLocal,
       bool isHostMessage,
       bool removeHeaders)
    {
        InitializeComponent();
        TableLayoutPanel.Controls.Add(ContentPanel, 0, 1);
        ContentPanel.Dock = DockStyle.Fill;

        this.isHostMessage = isHostMessage;
        this.isLocal = isLocal;
        this.removeHeaders = removeHeaders;

        FillHeaderLabels(name, isHostMessage ? "Хост" : string.Empty, time);
    }

    /// <summary>
    /// Является ли отправитель хостом
    /// </summary>
    readonly protected bool isHostMessage;

    /// <summary>
    /// Является ли отправитель сам пользователь
    /// </summary>
    readonly protected bool isLocal;

    /// <summary>
    /// Нужно ли убрать заголовок
    /// </summary>
    readonly protected bool removeHeaders;

    /// <summary>
    /// Цвет сообщения, в зависимости от отправителя
    /// </summary>
    protected Color SenderColor;

    /// <summary>
    /// Раскрасить всю карту этим цветом
    /// </summary>
    /// <param name="color"></param>
    protected void RecolorEntireCard(Color color)
    {
        var isDefault = SenderColor == color;

        foreach (Control control in Controls)
        {
            control.BackColor = color;
        }

        foreach (Control control in TableLayoutPanel.Controls)
        {
            control.BackColor = color;
            if (!isDefault)
            {
                control.ForeColor = ColorScheme.TextOnAccent;
            }
            else
            {
                control.ForeColor = ColorScheme.TextPrimary;
            }
        }
    }

    private void FillHeaderLabels(string name, string role, DateTime time)
    {
        senderName.Text = name;
        sendRole.Text = role;
        sendTime.Text = time.ToShortTimeString();
    }

    /// <inheritdoc />
    public virtual void ApplyStylings()
    {
        if (removeHeaders)
        {
            TableLayoutPanel.RowStyles[0].Height = 0;
            senderName.Visible = false;
            sendRole.Visible = false;
        }

        SenderColor = isLocal
            ? ColorScheme.OutgoingMessageBackground
            : ColorScheme.IncomingMessageBackground;

        senderName.BackColor = SenderColor;
        sendRole.BackColor = SenderColor;
        sendTime.BackColor = SenderColor;
        TableLayoutPanel.BackColor = SenderColor;

        senderName.Font = FontScheme.Monospace;
        sendRole.Font = FontScheme.MicroCaption;
        sendTime.Font = FontScheme.MicroCaption;
    }
}
