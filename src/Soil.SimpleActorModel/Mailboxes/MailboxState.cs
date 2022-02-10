using System;

namespace Soil.SimpleActorModel.Mailboxes;

public enum MailboxState
{
    Open = 0,

    Closed = 1,

    Scheduled = 2,
}
