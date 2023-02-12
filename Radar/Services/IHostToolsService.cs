using Radar.Common;

namespace Radar.Services
{
    public interface IHostToolsService
    {
        void ChooseService(IEnumerable<Host> hosts);
    }
}