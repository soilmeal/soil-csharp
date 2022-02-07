using Soil.Utils.Id;

namespace Soil.Net.Channel;

public class DefaultChannelIdGenerator : ObjectIdGenerator<IChannel, ChannelId>, IChannelIdGenerator
{
    protected override ChannelId CreateId(byte[] bytes)
    {
        return new ChannelId(bytes);
    }
}
