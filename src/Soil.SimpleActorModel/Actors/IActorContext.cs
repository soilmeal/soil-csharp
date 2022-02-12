using System;
using System.Collections.Generic;
using Soil.SimpleActorModel.Dispatcher;
using Soil.SimpleActorModel.Message;
using Soil.SimpleActorModel.Message.System;

namespace Soil.SimpleActorModel.Actors;

public interface IActorContext : IActorRef
{
    IActorRef Parent { get; }

    IActorRef Self { get; }

    IActorRef Sender { get; }

    IEnumerable<IActorRef> Children { get; }

    IDispatcher Dispatcher { get; }

    IActorRef Create(ActorProps props);

    void Invoke(Envelope envelope);

    void InvokeSystem(SystemMessage message);
}
