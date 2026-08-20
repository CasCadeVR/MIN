using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Core.Stores.Contracts.Interfaces;

namespace MIN.Core.Stores.Services;

/// <inheritdoc cref="IMessageStore"/>
public sealed class MessageStore : IMessageStore
{
    private readonly List<IMessage> messages = [];

    void IMessageStore.AddMessage(IMessage message, bool appendOnStart)
    {
        lock (messages)
        {
            if (appendOnStart)
            {
                messages.Insert(0, message);
            }
            else
            {
                messages.Add(message);
            }
        }
    }

    IMessage? IMessageStore.GetMessageById(Guid id)
        => messages.FirstOrDefault(p => p.Id == id);

    void IMessageStore.UpdateMessage(Guid id, IMessage message)
    {
        lock (messages)
        {
            var existing = messages.FirstOrDefault(p => p.Id == id);
            if (existing != null)
            {
                existing = message;
            }
        }
    }

    int IMessageStore.GetMessageCount()
    {
        lock (messages)
        {
            return messages.Count;
        }
    }

    IEnumerable<IMessage> IMessageStore.GetRecentHistory(int page, int pageSize)
    {
        lock (messages)
        {
            return messages
                .AsEnumerable()
                .Reverse()
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }
    }

    IEnumerable<IMessage> IMessageStore.GetHistory(int? page, int? pageSize)
    {
        lock (messages)
        {
            var resultMessages = messages.AsEnumerable().Reverse();

            if (page.HasValue && pageSize.HasValue)
            {
                resultMessages = messages.Skip(page.Value * pageSize.Value).Take((int)pageSize);
            }

            return resultMessages;
        }
    }

    IMessage IMessageStore.GetLastMessage()
    {
        lock (messages)
        {
            return messages.Last();
        }
    }

    void IMessageStore.RemoveMessage(Guid id)
    {
        lock (messages)
        {
            messages.RemoveAll(x => x.Id == id);
        }
    }

    void IMessageStore.ClearMessages()
    {
        lock (messages)
        {
            messages.Clear();
        }
    }
}
