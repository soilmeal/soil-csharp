using System;
using Soil.SimpleActorModel.Actors;

namespace Soil.SimpleActorModel.Message;

public class DefaultMailboxFactory : IMailboxFactory
{
    public Mailbox Create(IActorContext context, MailboxProps props)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        if (props == null)
        {
            throw new ArgumentNullException(nameof(props));
        }

        switch (props.Type)
        {
            case MailboxType.UnboundedMailboxType:
            {
                return new UnboundedMailbox(context);
            }
            default:
            {
                throw new ArgumentException("invalid dispatcher type", props.Type);
            }
        }
    }
}
