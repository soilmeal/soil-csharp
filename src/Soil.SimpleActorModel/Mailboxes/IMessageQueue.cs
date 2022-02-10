using System.Collections.Concurrent;

namespace Soil.SimpleActorModel.Mailboxes;

public interface IMessageQueue : IProducerConsumerCollection<Envelope>
{
    void Clear();
}
