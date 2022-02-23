namespace Soil.Net.Channel;

public class NoneChannelId : ChannelId
{
    public static readonly NoneChannelId Instance = new();

    private NoneChannelId()
        : base(new byte[sizeof(long)])
    {
    }
}
