using Soil.SimpleActorModel.Actors;

namespace Soil.SimpleActorModel.Message;

public interface IMailboxFactory
{
    Mailbox Create(IActorContext context, MailboxProps props);
}
