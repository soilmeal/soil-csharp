using Soil.SimpleActorModel.Actors;

namespace Soil.SimpleActorModel.Mailboxes;

public interface IMailboxProvider
{
    MailboxProps Props { get; }

    Mailbox Provide(IActorContext context);
}
