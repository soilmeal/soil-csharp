using System.Collections.Concurrent;
using Soil.SimpleActorModel.Message;

namespace Soil.SimpleActorModel.Message;

public interface IMessageQueue : IProducerConsumerCollection<Envelope>
{
    void Clear();
}
