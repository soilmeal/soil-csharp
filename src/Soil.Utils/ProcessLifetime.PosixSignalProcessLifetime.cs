#if NET6_0_OR_GREATER
using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace Soil.Utils;

public partial class ProcessLifetime
{
    private class PosixSignalProcessLifetime : IProcessLifetime
    {
        private readonly ManualResetEventSlim _sync = new(false);

        private readonly PosixSignalRegistration _sigint;

        private readonly PosixSignalRegistration _sigterm;

        public PosixSignalProcessLifetime()
        {
            _sigint = PosixSignalRegistration.Create(PosixSignal.SIGINT, HandleShutdownSignal);
            _sigterm = PosixSignalRegistration.Create(PosixSignal.SIGTERM, HandleShutdownSignal);
            Console.CancelKeyPress += HandleCancelKey;
        }

        public void NotifyProcessShutdownEnd()
        {
        }

        public void WaitForProcessShutdownStart()
        {
            _sync.Wait();
        }

        public void Dispose()
        {
            _sigint.Dispose();
            _sigterm.Dispose();
            GC.SuppressFinalize(this);
        }

        private void HandleShutdownSignal(PosixSignalContext ctx)
        {
            ctx.Cancel = true;

            _sync.Set();
        }

        private void HandleCancelKey(object? _, ConsoleCancelEventArgs args)
        {
            args.Cancel = true;

            _sync.Set();
        }
    }
}
#endif
