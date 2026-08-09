using MIN.Core.Stores.Contracts.Models;
using MIN.Voice.Messaging;
using MIN.Voice.Services.Contacts.Interfaces;

namespace MIN.Voice.Services;

/// <inheritdoc cref="IVoiceCallMessageResolver"/>
public class VoiceCallMessageResolver : IVoiceCallMessageResolver
{
    Guid? IVoiceCallMessageResolver.GetVoiceCallMessageIdOutOfSubRoomId(RoomContext context, int subRoomId)
    {
        var history = context.Messages.GetHistory();
        VoiceCallStartedMessage? match = null;

        foreach (var message in history)
        {
            if (message is VoiceCallStartedMessage voiceCallStartedMessage && subRoomId == voiceCallStartedMessage.SubRoomId)
            {
                match = voiceCallStartedMessage;
                break;
            }
        }

        return match?.Id;
    }
}
