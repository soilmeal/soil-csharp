using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Soil.SimpleActorModel.Dispatcher;
using Soil.SimpleActorModel.Message;
using Soil.SimpleActorModel.Message.System;

namespace Soil.SimpleActorModel.Actors;

public interface IActorContext : IActorRef, IEquatable<IActorContext>
{
    IActorRef Parent { get; }

    IActorRef Self { get; }

    IActorRef Sender { get; }

    IEnumerable<IActorRef> Children { get; }

    IDispatcher Dispatcher { get; }

    Mailbox Mailbox { get; }

    IActorContext Create(ActorProps props);

    void Start();

    Task StartAsync();

    void Stop(bool waitChildren);

    Task StopAsync(bool waitChildren);

    void Invoke(Envelope envelope);

    void InvokeSystem(SystemMessage message);
}
