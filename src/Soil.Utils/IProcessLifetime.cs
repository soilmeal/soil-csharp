using System;
using System.Threading.Tasks;

namespace Soil.Utils;

public interface IProcessLifetime : IDisposable
{
    public void WaitForProcessShutdownStart();

    public void NotifyProcessShutdownEnd();
}
