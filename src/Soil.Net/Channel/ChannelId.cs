using Soil.Utils.Id;

namespace Soil.Net.Channel;

public class ChannelId : ObjectId
{
    public ChannelId(byte[] bytes)
        : base(bytes)
    {
    }
}
