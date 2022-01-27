using System;

namespace Soil.Utils;

public partial class ProcessLifetime : IProcessLifetime
{
    private readonly IProcessLifetime _impl;

    public ProcessLifetime()
    {
#if NET6_0_OR_GREATER
        _impl = new PosixSignalProcessLifetime();
#else
        _impl = new AppDomainProcessLifetime();
#endif
    }

    public void WaitForProcessShutdownStart()
    {
        _impl.WaitForProcessShutdownStart();
    }

    public void NotifyProcessShutdownEnd()
    {
        _impl.NotifyProcessShutdownEnd();
    }

    public void Dispose()
    {
        _impl.Dispose();
        GC.SuppressFinalize(this);
    }
}
