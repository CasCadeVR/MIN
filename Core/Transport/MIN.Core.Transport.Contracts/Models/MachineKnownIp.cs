using System.Net;
using MIN.Core.Transport.Contracts.Enum;

namespace MIN.Core.Transport.Contracts.Models;

/// <summary>
/// Понятная компьютеру IP адрес и его происходение
/// </summary>
public record MachineKnownIp(IPAddress Address, AddressOrigin Origin, string? NetworkName = null);
