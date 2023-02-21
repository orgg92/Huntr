namespace Radar.Services.Interfaces
{
    using Radar.Common.NetworkModels;

    public interface IHostToolsService
    {
        void ChooseService(IEnumerable<Host> hosts);
    }
}