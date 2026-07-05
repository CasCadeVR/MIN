using System;
using System.Linq;
using System.Net;

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
        else
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Валидировать IP адрес и вернуть ip адрес в случае DNS резолвинга
    /// </summary>
    public static string ValidateIP(string ipAddress)
    {
        if (!IPAddress.TryParse(ipAddress, out _))
        {
            try
            {
                var iPHostEntry = Dns.GetHostEntry(ipAddress);
                if (iPHostEntry.AddressList.Length == 0)
                {
                    throw new InvalidOperationException("IP Адрес задан в неккоретном формате");
                }
                else
                {
                    return iPHostEntry.AddressList.First().ToString();
                }
            }
            catch (Exception)
            {
                throw new InvalidOperationException("DNS не смог распознать IP адрес");
            }
        }
        else
        {
            return ipAddress;
        }
    }
}
