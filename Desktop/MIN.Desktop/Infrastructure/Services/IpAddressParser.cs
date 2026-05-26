namespace MIN.Desktop.Infrastructure.Services;

/// <summary>
/// Помошник в парсировании ip адреса на адрес и порт
/// </summary>
public static class IpAddressParser
{
    /// <summary>
    /// Распарсить ip адрес на адрес и порт
    /// </summary>
    public static bool TryParseIpAddress(string? ipAddressAndPort, out string ipAddress, out int port)
    {
        ipAddress = string.Empty;
        port = 0;
        var input = ipAddressAndPort;
        if (string.IsNullOrWhiteSpace(input))
        {
            return false;
        }

        var parts = input.Split(':');
        if (parts.Length == 2 && int.TryParse(parts[1], out var parsedPort) && parsedPort > 0 && parsedPort <= 65535)
        {
            port = parsedPort;
            ipAddress = parts[0];
        }

        return true;
    }
}
