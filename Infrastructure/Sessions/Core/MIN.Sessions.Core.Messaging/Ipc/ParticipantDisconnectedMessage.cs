using MIN.Sessions.Core.Messaging.Contracts.Enums;
using MIN.Sessions.Core.Messaging.Contracts.Models;

namespace MIN.Sessions.Core.Messaging.Ipc;

/// <inheritdoc cref="IpcMessageType.ParticipantDisconnected"/>
public sealed record ParticipantDisconnectedMessage(string ParticipantId) : IpcMessage(IpcMessageType.ParticipantDisconnected);
