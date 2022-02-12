using System;
using System.Threading.Tasks;

namespace Soil.SimpleActorModel.Dispatcher;

public interface IExecutor : IDisposable
{
    Task Execute(Action action);

    void JoinAll();

    void JoinAll(TimeSpan timeout);

    void JoinAll(int millisecondsTimeout);
}
