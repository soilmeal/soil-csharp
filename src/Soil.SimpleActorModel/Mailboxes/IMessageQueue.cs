using System.Collections.Concurrent;
using Soil.SimpleActorModel.Messages;

namespace Soil.SimpleActorModel.Mailboxes;

public interface IMessageQueue : IProducerConsumerCollection<Envelope>
{
    void Clear();
}
