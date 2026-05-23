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
    /// Инициализирует новый экземпляр <see cref="TcpDirectConnectForm"/>
    /// </summary>
    public TcpDirectConnectForm()
    {
        InitializeComponent();

        Shown += (_, _) => port.Focus();
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
            throw new InvalidOperationException("IP Адрес задан в неккоретном формате");
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
        Endpoint.Port = Convert.ToInt32(port.Value);

        DialogResult = DialogResult.OK;
    }
}
