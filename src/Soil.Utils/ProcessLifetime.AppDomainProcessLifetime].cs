#if !NET6_0_OR_GREATER
using System;
using System.Threading;

namespace Soil.Utils;

public partial class ProcessLifetime
{
    private class AppDomainProcessLifetime : IProcessLifetime
    {
        private readonly ManualResetEventSlim _syncStart = new(false);
        private readonly ManualResetEventSlim _syncEnd = new(false);

        public AppDomainProcessLifetime()
        {
            AppDomain.CurrentDomain.ProcessExit += HandleProcessExit;
            Console.CancelKeyPress += HandleCancelKey;
        }

        public void NotifyProcessShutdownEnd()
        {
            _syncEnd.Set();
        }

        public void WaitForProcessShutdownStart()
        {
            _syncStart.Wait();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        private void HandleProcessExit(object? sender, EventArgs args)
        {
            _syncStart.Set();

            _syncEnd.Wait();
        }

        private void HandleCancelKey(object? sender, ConsoleCancelEventArgs args)
        {
            args.Cancel = true;

            _syncStart.Set();
        }
    }
}
#endif
