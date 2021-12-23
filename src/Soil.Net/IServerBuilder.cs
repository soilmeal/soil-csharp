using System;
using System.Net;
using Soil.Core.Threading;
using Soil.Net.Channel;

namespace Soil.Net;

public interface IServerBuilder<TSelf, TServer, TChannel>
        where TSelf : IServerBuilder<TSelf, TServer, TChannel>
        where TServer : IServer<TChannel>
        where TChannel : IChannel
{
    IPAddress IPAddress { get; }

    int Port { get; }

    int Backlog { get; }

    int MasterThreadCount { get; }

    IThreadFactory? MasterThreadFactory { get; }

    int WorkerThreadCount { get; }

    IThreadFactory? WorkerThreadFactory { get; }

    public TSelf SetAddress(string ipString);

    public TSelf SetAddress(ReadOnlySpan<char> ipSpan);

    public TSelf SetAddress(long address);

    public TSelf SetAddress(byte[] address);

    public TSelf SetAddress(ReadOnlySpan<byte> address);

    public TSelf SetAddress(byte[] address, long scopeid);

    public TSelf SetAddress(ReadOnlySpan<byte> address, long scopeid);

    public TSelf SetAddress(IPAddress ipAddress);

    public TSelf SetPort(int port);

    public TSelf SetBacklog(int backlog);

    public TSelf SetMasterThreadCount(int masterThreadCount);

    public TSelf SetMasterThreadFactory(IThreadFactory threadFactory);

    public TSelf SetWorkerThreadCount(int workerThreadCount);

    public TSelf SetWorkerThreadFactory(IThreadFactory threadFactory);

    public TServer Build();
}
