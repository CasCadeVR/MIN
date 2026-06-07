using MIN.Sessions.Core.Messaging.Contracts.Enums;

namespace MIN.Sessions.Core.Messaging.Contracts.Models;

/// <summary>
/// Между-процессорное сообщение
/// </summary>
public abstract record IpcMessage(IpcMessageType Type);
