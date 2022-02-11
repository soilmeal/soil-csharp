using Soil.SimpleActorModel.Actors;

namespace Soil.SimpleActorModel.Mailboxes;

public class DefaultMailboxFactory : IMailboxFactory
{
    public Mailbox Create(MailboxProps props, IActorContext context)
    {
        switch (props.Type)
        {
            case DefaultMailboxType.UnboundedMailboxType:
            {
                return new UnboundedMailbox(context);
            }
            default:
            {
                return Mailboxes.None;
            }
        }
    }
}
