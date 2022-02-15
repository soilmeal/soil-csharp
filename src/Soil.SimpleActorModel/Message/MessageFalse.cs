namespace Soil.SimpleActorModel.Message;

public class MessageFalse : IMessageBool
{
    public static MessageFalse Instance = new();

    public bool Value
    {
        get
        {
            return false;
        }
    }

    private MessageFalse()
    {
    }
}
