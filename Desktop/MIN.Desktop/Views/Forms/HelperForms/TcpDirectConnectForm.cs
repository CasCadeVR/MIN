using System.Net;
using MIN.Core.Transport.TcpSockets.Models;
using MIN.Desktop.Contracts.Schemes;
using MIN.Desktop.Contracts.Views.Forms;

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

    private void ValidateIP()
    {
        if (!IPAddress.TryParse(ipAddress.Text, out _))
        {
            try
            {
                var iPHostEntry = Dns.GetHostEntry(ipAddress.Text);
                if (iPHostEntry.AddressList.Length == 0)
                {
                    throw new InvalidOperationException("IP Адрес задан в неккоретном формате");
                }
                else
                {
                    ipAddress.Text = iPHostEntry.AddressList.First().ToString();
                }
            }
            catch (Exception)
            {
                throw new InvalidOperationException("DNS не смог распознать IP адрес");
            }
        }
    }

    private void connectButton_Click(object sender, EventArgs e)
    {
        try
        {
            ValidateIP();
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
        var input = ipAddress.Text;
        if (string.IsNullOrWhiteSpace(input))
        {
            return;
        }

        var parts = input.Split(':');
        if (parts.Length == 2 && int.TryParse(parts[1], out var port) && port > 0 && port <= 65535)
        {
            portNumericUpDown.Value = port;
            ipAddress.Text = parts[0];
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
