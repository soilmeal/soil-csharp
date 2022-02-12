using Soil.SimpleActorModel.Actors;

namespace Soil.SimpleActorModel.Message;

public struct Envelope
{
    private readonly object _message;

    private readonly IActorRef _sender;

    public object Message
    {
        get
        {
            return _message;
        }
    }

    public IActorRef Sender
    {
        get
        {
            return _sender;
        }
    }

    public Envelope(object message, IActorRef sender)
    {
        _message = message;
        _sender = sender;
    }
}
