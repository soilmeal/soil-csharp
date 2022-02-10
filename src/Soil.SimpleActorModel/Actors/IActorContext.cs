using System.Collections.Generic;
using Soil.SimpleActorModel.Dispatchers;

namespace Soil.SimpleActorModel.Actors;

public interface IActorContext
{
    IActorRef Parent { get; }

    IActorRef Self { get; }

    IActorRef Sender { get; }

    IEnumerable<IActorRef> Children { get; }

    IDispatcher Dispatcher { get; }

    IActorRef Create(AbstractActor actor);
}
