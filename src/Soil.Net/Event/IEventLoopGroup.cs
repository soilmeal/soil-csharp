
using System.Collections.Generic;

namespace Soil.Net.Event;

public interface IEventLoopGroup : IEnumerator<IEventLoop>, IEventSourceRegistry
{
    IEventLoop Next();
}
