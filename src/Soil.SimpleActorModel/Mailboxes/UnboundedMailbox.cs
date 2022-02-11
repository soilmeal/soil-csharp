using Soil.SimpleActorModel.Actors;

namespace Soil.SimpleActorModel.Mailboxes;

public class UnboundedMailbox : Mailbox
{
    public UnboundedMailbox(IActorContext owner)
        : base(owner, new UnboundedMessageQueue())
    {
    }
}