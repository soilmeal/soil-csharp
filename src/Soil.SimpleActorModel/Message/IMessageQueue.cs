using System.Collections.Concurrent;

namespace Soil.SimpleActorModel.Message;

public interface IMessageQueue : IProducerConsumerCollection<Envelope>
{
    void Clear();
}
