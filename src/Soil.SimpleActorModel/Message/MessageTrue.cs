namespace Soil.SimpleActorModel.Message;

public class MessageTrue : IMessageBool
{
    public static MessageTrue Instance = new();

    public bool Value
    {
        get
        {
            return true;
        }
    }

    private MessageTrue()
    {
    }
}
