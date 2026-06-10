using MIN.Sessions.Core.Messaging.Contracts.Enums;
using MIN.Sessions.Core.Messaging.Contracts.Models;

namespace MIN.Sessions.Core.Messaging.Ipc;

/// <inheritdoc cref="IpcMessageType.ServerShutdown"/>
public sealed record ServerShutdownMessage(string Reason) : IpcMessage(IpcMessageType.ServerShutdown);
