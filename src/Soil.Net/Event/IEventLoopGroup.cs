
using System.Collections.Generic;

namespace Soil.Net.Event;

public interface IEventLoopGroup : IEventLoop
{
    IEventLoop Next();
}
