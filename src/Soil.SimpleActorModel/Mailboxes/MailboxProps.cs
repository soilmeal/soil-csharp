namespace Soil.SimpleActorModel.Mailboxes;

public class MailboxProps
{
    private readonly string _type;

    public string Type
    {
        get
        {
            return _type;
        }
    }

    public MailboxProps(string type)
    {
        _type = type;
    }
}
