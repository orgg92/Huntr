namespace Radar.Services.Interfaces
{
    using Radar.Common.NetworkModels;

    public interface ISSHService
    {
        bool AttemptConnection(Host host);
    }
}