using System.Collections.Concurrent;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Core.Services.Contracts.Interfaces.Stores;

namespace MIN.Core.Services.Stores
{
    /// <inheritdoc cref="IMessageStore"/>
    public sealed class MessageStore : IMessageStore
    {
        private readonly ConcurrentDictionary<Guid, List<IMessage>> messages = new();

        void IMessageStore.AddMessage(Guid roomId, IMessage message)
        {
            var messages = this.messages.GetOrAdd(roomId, _ => []);
            lock (messages)
            {
                if (!messages.Any(x => x.Id == message.Id))
                {
                    messages.Add(message);
                }
            }
        }

        IEnumerable<IMessage> IMessageStore.GetHistory(Guid roomId, int? page, int? pageSize)
        {
            if (messages.TryGetValue(roomId, out var roomMessages))
            {
                lock (roomMessages)
                {
                    var resultMessages = roomMessages.ToList();

                    if (page.HasValue)
                    {
                        resultMessages = roomMessages.Skip((int)page).ToList();
                    }

                    if (pageSize.HasValue)
                    {
                        resultMessages = roomMessages.Take((int)pageSize).ToList();
                    }

                    return resultMessages;
                }
            }

            return [];
        }

        void IMessageStore.ClearMessages(Guid roomId)
        {
            messages.TryRemove(roomId, out _);
        }
    }
}
