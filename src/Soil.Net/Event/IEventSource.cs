using System;
using System.Threading.Tasks;

namespace Soil.Net.Event;

public interface IEventSource
{
    public delegate void RegistrationHandler();

    public delegate void DeregistrationHandler();

    IEventLoop? EventLoop { get; }

    void Register(IEventLoop eventLoop, Action? action = default);

    void Register(IEventLoopGroup eventLoopGroup, Action? action = default);

    Task RegisterAsync(IEventLoop eventLoop);

    Task RegisterAsync(IEventLoopGroup eventLoopGroup);

    void Deregister(Action? action = default);

    Task DeregisterAsync();


}
