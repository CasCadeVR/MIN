using MIN.Core.Transport.TcpSockets.Models;
using MIN.Desktop.Contracts.Schemes;
using MIN.Desktop.Contracts.Views.Forms;
using MIN.Desktop.Infrastructure.Services;

namespace MIN.Desktop.Views.Forms.HelperForms;

/// <summary>
/// Форма создания комнаты
/// </summary>
public partial class TcpDirectConnectForm : StyledForm
{
    /// <summary>
    /// Полученная конечная точка
    /// </summary>
    public TcpEndpoint Endpoint { get; set; } = new();

    /// <summary>
    /// Событие по нажатию на кнопку
    /// </summary>
    public Action? OnConnect { get; set; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="TcpDirectConnectForm"/>
    /// </summary>
    public TcpDirectConnectForm()
    {
        InitializeComponent();

        Shown += (_, _) => ipAddress.Focus();
    }

    /// <summary>
    /// Включить кнопку подключения обратно
    /// </summary>
    public void EnableConnectButton()
    {
        connectButton.Enabled = true;
    }

    /// <inheritdoc />
    protected override void ApplyStylings()
    {
        splitContainer.Panel1.BackColor = ColorScheme.PrimaryAccent;
        splitContainer.Panel2.BackColor = ColorScheme.MainPanelBackground;
        Title.ForeColor = ColorScheme.TextOnAccent;
    }

    private void connectButton_Click(object sender, EventArgs e)
    {
        try
        {
            ipAddress.Text = IpAddressParser.ValidateIP(ipAddress.Text);
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

        Endpoint.IPAddress = ipAddress.Text;
        Endpoint.Port = Convert.ToInt32(portNumericUpDown.Value);

        connectButton.Enabled = false;
        OnConnect?.Invoke();
    }

    private void cancelButton_Click(object sender, EventArgs e)
    {
        Close();
    }

    private void ipAddress_Leave(object sender, EventArgs e)
    {
        TryParsePort();
    }

    private void TryParsePort()
    {
        if (IpAddressParser.TryParseIpAddress(ipAddress.Text, out var gottenIpAddress, out var port))
        {
            portNumericUpDown.Value = port;
            ipAddress.Text = gottenIpAddress;
        }
    }

    private void ipAddress_KeyPress(object sender, KeyPressEventArgs e)
    {
        if (e.KeyChar == '\r')
        {
            TryParsePort();
        }
    }
}
