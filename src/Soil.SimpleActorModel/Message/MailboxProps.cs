using System;

namespace Soil.SimpleActorModel.Message;

public class MailboxProps
{
    private string _type;

    public string Type
    {
        get
        {
            return _type;
        }
    }

    public MailboxProps()
    {
        _type = Mailboxes.DefaultType;
    }

    public MailboxProps SetType(string type)
    {
        if (string.IsNullOrEmpty(type))
        {
            throw new ArgumentException($"{nameof(type)} is null or empty", type);
        }

        _type = type;

        return this;
    }
}
