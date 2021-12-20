using System;
using System.Net;
using System.Net.Sockets;

namespace Soil.Net;

public class TcpServer : IServer<TcpClient>
{
    private readonly IPEndPoint _localEndPoint;

    public IPEndPoint LocalEndPoint { get { return _localEndPoint; } }

    private readonly int _backlog;

    public int Backlog { get { return _backlog; } }

    private readonly ClientGroup<TcpClient> _groups;

    private readonly TcpListener _listener;

    private TcpServer(IPEndPoint localEndPoint_, int backlog_)
    {
        _localEndPoint = localEndPoint_;
        _backlog = backlog_;

        _listener = new TcpListener(_localEndPoint);
    }

    public void Start()
    {
        _listener.Start(_backlog);
    }

    public bool Pending()
    {
        return _listener.Pending();
    }

    public void Stop()
    {
        _listener.Stop();
    }

    public class Builder
    {
        private IPAddress _ipAddress = IPAddress.None;

        private int _port = IPEndPoint.MinPort;

        private int _backlog = 1;

        public Builder Address(string? ipString)
        {
            if (ipString == null)
            {
                throw new ArgumentNullException(nameof(ipString));
            }

            _ipAddress = IPAddress.Parse(ipString);

            return this;
        }

        public Builder Port(int port_)
        {
            if (port_ < IPEndPoint.MinPort || port_ > IPEndPoint.MaxPort)
            {
                throw new ArgumentOutOfRangeException(nameof(port_), port_, $"{nameof(port_)} is must be in between {IPEndPoint.MinPort} and {IPEndPoint.MaxPort}.");
            }

            _port = port_;

            return this;
        }

        public Builder Backlog(int backlog_)
        {

            if (backlog_ < 0)
            {
                throw new ArgumentException($"{nameof(backlog_)} is must not be zero.", nameof(backlog_));
            }
            _backlog = backlog_;

            return this;
        }

        public IServer<TcpClient> Build()
        {
            IPEndPoint ipEndPoint = new IPEndPoint(_ipAddress, _port);

            return new TcpServer(ipEndPoint, _backlog);
        }
    }
}
