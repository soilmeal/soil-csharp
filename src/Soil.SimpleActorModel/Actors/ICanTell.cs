using Soil.SimpleActorModel.Actors;

namespace Soil.SimpleActorModel.Actors;

public interface ICanTell
{
    void Tell(object? message);

    void Tell(object? message, IActorRef sender);
}
