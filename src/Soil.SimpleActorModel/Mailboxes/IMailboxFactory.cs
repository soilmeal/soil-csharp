using Soil.SimpleActorModel.Actors;

namespace Soil.SimpleActorModel.Mailboxes;

public interface IMailboxFactory
{
    Mailbox Create(MailboxProps props, IActorContext context);
}
