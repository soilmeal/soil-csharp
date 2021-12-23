using System.Threading;
using System.Threading.Tasks;

namespace Soil.Net.Channel;

public interface IChannelGroup<TChannel>
    where TChannel : IChannel
{
    Task<bool> Register(TChannel? channel);

    Task<bool> Register(TChannel? channel, CancellationToken cancellationToken);

    Task<bool> Register(Task<TChannel> channelTask);

    Task<bool> Register(Task<TChannel> channelTask, CancellationToken cancellationToken);
}
