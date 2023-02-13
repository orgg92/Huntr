using Radar.Common.NetworkModels;

namespace Radar.Services.Interfaces
{
    public interface IHostToolsService
    {
        void ChooseService(IEnumerable<Host> hosts);
    }
}