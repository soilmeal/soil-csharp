using Soil.Utils.Id;

namespace Soil.Net.Channel;

public class ChannelId : HashCodeBasedId
{
    public ChannelId(byte[] bytes)
        : base(bytes)
    {
    }
}
