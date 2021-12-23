using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Soil.Core.Threading;
using Soil.Core.Threading.Atomic;
using Soil.Net.Channel;
using Soil.Net.Threading.Tasks;

namespace Soil.Net;

public class TcpServer : IServer<TcpChannel>
{
    private readonly IPEndPoint _localEndPoint;

    public IPEndPoint LocalEndPoint { get { return _localEndPoint; } }

    private readonly int _backlog;

    public int Backlog { get { return _backlog; } }

    // private readonly int _masterThreadCount;

    private readonly ITaskSchedulerGroup _schedulerGroup;

    // private readonly int _workerThreadCount;

    private readonly IChannelGroup<TcpChannel> _channelGroup;

    private readonly AtomicInt32 _status;
    public ServerStatus Status
    {
        get
        {
            return (ServerStatus)_status.Read();
        }
    }

    private readonly TcpListener _listener;

    private TcpServer(
        IPEndPoint localEndPoint,
        int backlog,
        int masterThreadCount,
        IThreadFactory masterThreadFactoy,
        int workerThreadCount,
        IThreadFactory workerThreadFactory)
    {
        _localEndPoint = localEndPoint;
        _backlog = backlog;

        // _masterThreadCount = masterThreadCount;
        _schedulerGroup = (masterThreadCount <= 1)
            ? new SingleTaskSchedulerGroup(masterThreadFactoy)
            : new MultiTaskSchedulerGroup(masterThreadCount, masterThreadFactoy);

        // _workerThreadCount = workerThreadCount;
        _channelGroup = (workerThreadCount <= 1)
            ? new SingleThreadChannelGroup<TcpChannel>(workerThreadFactory)
            : new MultiThreadChannelGroup<TcpChannel>(workerThreadCount, workerThreadFactory);


        _status = new AtomicInt32((int)ServerStatus.None);
        _listener = new TcpListener(_localEndPoint);
    }

    public bool Pending()
    {
        return _listener.Pending();
    }

    public void Start()
    {
        ServerStatus oldStatus = (ServerStatus)_status.CompareExchange(
            (int)ServerStatus.Starting,
            (int)ServerStatus.None);
        if (oldStatus != ServerStatus.None)
        {
            return;
        }

        _listener.Start(_backlog);

        _schedulerGroup.StartNewOnNextScheduler(
            Accept,
            TaskCreationOptions.HideScheduler
            | TaskCreationOptions.DenyChildAttach);
    }

    public void Stop()
    {
        _listener.Stop();
    }

    private void Accept()
    {
        ServerStatus oldStatus = (ServerStatus)_status.CompareExchange(
            (int)ServerStatus.Listening,
            (int)ServerStatus.Starting);
        if (oldStatus != ServerStatus.Starting)
        {
            return;
        }

        while (Status == ServerStatus.Listening)
        {
            TcpClient client;
            try
            {
                client = _listener.AcceptTcpClient();
            }
            catch (InvalidOperationException)
            {
                // TODO: NEED LOG
                break;
            }
            catch (SocketException)
            {
                // TODO: NEED LOG
                continue;
            }

            _channelGroup.Register(new TcpChannel(client));
        }
    }

    public class Builder : IServerBuilder<Builder, TcpServer, TcpChannel>
    {
        private IPAddress _ipAddress = IPAddress.None;
        public IPAddress IPAddress
        {
            get
            {
                return _ipAddress;
            }
        }

        private int _port = -1;
        public int Port
        {
            get
            {
                return _port;
            }
        }

        private int _backlog = 0;
        public int Backlog
        {
            get
            {
                return _backlog;
            }
        }

        private int _masterThreadCount = 0;
        public int MasterThreadCount
        {
            get
            {
                return _masterThreadCount;
            }
        }

        private IThreadFactory? _masterThreadFactory = null;
        public IThreadFactory? MasterThreadFactory
        {
            get
            {
                return _masterThreadFactory;
            }
        }

        private int _workerThreadCount = 0;
        public int WorkerThreadCount
        {
            get
            {
                return _workerThreadCount;
            }
        }

        private IThreadFactory? _workerThreadFactory = null;
        public IThreadFactory? WorkerThreadFactory
        {
            get
            {
                return _workerThreadFactory;
            }
        }

        public Builder SetAddress(string ipString)
        {
            return ipString != null
                ? SetAddress(IPAddress.Parse(ipString))
                : throw new ArgumentNullException(nameof(ipString));
        }

        public Builder SetAddress(ReadOnlySpan<char> ipSpan)
        {
            return SetAddress(IPAddress.Parse(ipSpan));
        }

        public Builder SetAddress(long address)
        {
            return SetAddress(new IPAddress(address));
        }

        public Builder SetAddress(byte[] address)
        {
            return address != null
                ? SetAddress(new IPAddress(address))
                : throw new ArgumentNullException(nameof(address));
        }

        public Builder SetAddress(ReadOnlySpan<byte> address)
        {
            return SetAddress(new IPAddress(address));
        }

        public Builder SetAddress(byte[] address, long scopeid)
        {
            return address != null
                ? SetAddress(new IPAddress(address))
                : throw new ArgumentNullException(nameof(address));
        }

        public Builder SetAddress(ReadOnlySpan<byte> address, long scopeid)
        {
            return SetAddress(new IPAddress(address, scopeid));
        }

        public Builder SetAddress(IPAddress ipAddress)
        {
            if (ipAddress == null || ipAddress == IPAddress.None)
            {
                throw new ArgumentNullException(nameof(ipAddress));
            }

            _ipAddress = ipAddress;
            return this;
        }

        public Builder SetPort(int port)
        {
            if (port < IPEndPoint.MinPort || port > IPEndPoint.MaxPort)
            {
                throw new ArgumentOutOfRangeException(nameof(port), port, $"{nameof(port)} is must be in between {IPEndPoint.MinPort} and {IPEndPoint.MaxPort}.");
            }

            _port = port;
            return this;
        }

        public Builder SetBacklog(int backlog)
        {
            _backlog = backlog;
            return this;
        }

        public Builder SetMasterThreadCount(int masterThreadCount)
        {
            _masterThreadCount = masterThreadCount;
            return this;
        }

        public Builder SetMasterThreadFactory(IThreadFactory threadFactory)
        {
            _masterThreadFactory = threadFactory ?? throw new ArgumentNullException(nameof(threadFactory));
            return this;
        }

        public Builder SetWorkerThreadCount(int workerThreadCount)
        {
            _workerThreadCount = workerThreadCount;
            return this;
        }

        public Builder SetWorkerThreadFactory(IThreadFactory threadFactory)
        {
            _workerThreadFactory = threadFactory ?? throw new ArgumentNullException(nameof(threadFactory));
            return this;
        }

        public TcpServer Build()
        {

            IPAddress ipAddress = _ipAddress != IPAddress.None
                ? _ipAddress
                : throw new InvalidOperationException("Require to set IPAddress");

            int port = _port >= 0
                ? _port
                : throw new InvalidOperationException("Require to set Port");

            IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, port);
            int backlog = GetOrDefaultBacklog();
            int masterThreadCount = GetOrDefaultMasterThreadCount();
            int workerThreadCount = GetOrDefaultWorkerThreadCount();
            IThreadFactory masterThreadFactory = GetOrDefaultMasterThreadFactory();
            IThreadFactory workerThreadFactory = GetOrDefaultWorkerThreadFactory();

            return new TcpServer(
                ipEndPoint,
                backlog,
                masterThreadCount,
                masterThreadFactory,
                workerThreadCount,
                workerThreadFactory);
        }

        private int GetOrDefaultBacklog()
        {
            int backlog = _backlog;
            return backlog > 0 ? backlog : 1024;
        }

        private int GetOrDefaultMasterThreadCount()
        {
            int masterThreadCount = _masterThreadCount;
            return masterThreadCount > 0 ? masterThreadCount : 1;
        }

        private IThreadFactory GetOrDefaultMasterThreadFactory()
        {
            return _masterThreadFactory ?? ThreadFactoryBuilder.BuildDefault();
        }

        private int GetOrDefaultWorkerThreadCount()
        {
            int workerThreadCount = _workerThreadCount;
            return workerThreadCount > 0 ? workerThreadCount : Environment.ProcessorCount;
        }

        private IThreadFactory GetOrDefaultWorkerThreadFactory()
        {
            return _workerThreadFactory ?? ThreadFactoryBuilder.BuildDefault();
        }
    }
}
