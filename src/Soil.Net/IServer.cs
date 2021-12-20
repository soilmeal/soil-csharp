namespace Soil.Net;

public interface IServer<TClient> where TClient : IClient
{
    void Start();

    void Stop();
}
