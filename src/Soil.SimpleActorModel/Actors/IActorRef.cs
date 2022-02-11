using System;
using System.Threading.Tasks;

namespace Soil.SimpleActorModel.Actors;

public interface IActorRef : IEquatable<IActorRef>
{
    void Start();

    void Stop();

    void Send(object message);

    void Send(object message, IActorRef sender);
}
