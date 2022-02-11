using System;
using System.IO;
using Soil.SimpleActorModel.Actors;

namespace Soil.SimpleActorModel.Mailboxes;

public class DefaultMailboxProvider : IMailboxProvider
{
    private readonly MailboxProps _props;

    private readonly DefaultMailboxFactory _factory = new();

    public MailboxProps Props
    {
        get
        {
            return _props;
        }
    }

    public DefaultMailboxProvider(MailboxProps props)
    {
        _props = props;
    }

    public Mailbox Provide(IActorContext context)
    {
        return _factory.Create(_props, context);
    }
}
