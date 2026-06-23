using MIN.Sessions.Core.Messaging.Contracts.Enums;
using MIN.Sessions.Core.Messaging.Contracts.Models;

namespace MIN.Sessions.Core.Messaging.Ipc;

/// <inheritdoc cref="IpcMessageType.ParticipantConnected"/>
public sealed record ParticipantConnectedMessage(string ParticipantId,
    string Name
) : IpcMessage(IpcMessageType.ParticipantConnected);
