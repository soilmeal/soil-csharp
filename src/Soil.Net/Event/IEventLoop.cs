using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Soil.Net.Event;

[SuppressMessage("Design", "CA1068: CancellationToken parameters must come last", Justification = "Adjust to signature of TaskFactory")]
public interface IEventLoop : IEventSourceRegistry
{
    TaskFactory TaskFactory { get; }

    bool InitSynchronizationContextAlways { get; }
}
