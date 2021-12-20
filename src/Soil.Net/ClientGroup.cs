using System.Collections.Generic;
using Soil.Core.Threading.Tasks;

namespace Soil.Net;

public class ClientGroup<TClient> where TClient : IClient
{
    private readonly TaskScheduler _taskScheduler;

    private readonly Dictionary<ulong, TClient> _clients = new Dictionary<ulong, TClient>();

    public ClientGroup()
    {
    }
}
