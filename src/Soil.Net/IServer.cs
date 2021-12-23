using Soil.Net.Channel;

namespace Soil.Net;

public interface IServer<TChannel>
    where TChannel : IChannel
{
    void Start();

    void Stop();
}
