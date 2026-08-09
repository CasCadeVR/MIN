using MIN.Core.Stores.Contracts.Models;
using MIN.Sessions.Core.Messaging.OutOfSubRoom;
using MIN.Sessions.Core.Services.Contracts.Interfaces;

namespace MIN.Sessions.Core.Services;

/// <summary>
/// <inheritdoc cref="ISessionReadyMessageResolver"/>
/// </summary>
public class SessionReadyMessageResolver : ISessionReadyMessageResolver
{
    Guid? ISessionReadyMessageResolver.GetSessionReadyMessageIdOutOfSubRoomId(RoomContext context, int subRoomId)
    {
        var history = context.Messages.GetHistory();
        SessionReadyMessage? match = null;

        foreach (var message in history)
        {
            if (message is SessionReadyMessage sessionReadyMessage && subRoomId == sessionReadyMessage.SubRoomId)
            {
                match = sessionReadyMessage;
                break;
            }
        }

        return match?.Id;
    }
}
