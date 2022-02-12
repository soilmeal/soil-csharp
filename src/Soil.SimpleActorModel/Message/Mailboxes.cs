using Soil.SimpleActorModel.Actors;

namespace Soil.SimpleActorModel.Message;

public static class Mailboxes
{
    public const string DefaultType = "unbounded-mailbox";

    public static readonly Mailbox None = new NoneMailbox();

    private class NoneMailbox : Mailbox
    {
        public NoneMailbox()
            : base(ActorContexts.None, MessageQueues.None)
        {
        }
    }
}
