using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Core.Stores.Contracts.Interfaces;

namespace MIN.Core.Stores.Services;

/// <inheritdoc cref="IMessageStore"/>
public sealed class MessageStore : IMessageStore
{
    private readonly List<IMessage> messages = [];

    void IMessageStore.AddMessage(IMessage message)
    {
        lock (messages)
        {
            messages.Add(message);
        }
    }

    IEnumerable<IMessage> IMessageStore.GetHistory(int? page, int? pageSize)
    {
        lock (messages)
        {
            var resultMessages = messages.ToList();

            if (page.HasValue)
            {
                resultMessages = messages.Skip((int)page).ToList();
            }

            if (pageSize.HasValue)
            {
                resultMessages = messages.Take((int)pageSize).ToList();
            }

            return resultMessages;
        }
    }

    void IMessageStore.ClearMessages()
    {
        messages.Clear();
    }
}
