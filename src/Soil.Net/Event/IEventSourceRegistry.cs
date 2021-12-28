using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Soil.Net.Channel;

namespace Soil.Net.Event;

public interface IEventSourceRegistry
{
    void Register(IChannel channel);

    Task RegisterAsync(IChannel channel);
}
