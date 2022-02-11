using Soil.SimpleActorModel.Actors;

namespace Soil.SimpleActorModel.Mailboxes;

public static class Mailboxes
{
    public static readonly Mailbox None = new NoneMailbox();

    private class NoneMailbox : Mailbox
    {
        public NoneMailbox()
            : base(ActorContexts.None, MessageQueues.None)
        {
        }
    }
}
