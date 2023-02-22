namespace Radar.Services.Interfaces
{
    using Radar.Common.Config;
    using Radar.Common.NetworkModels;

    public interface IHostToolsService
    {
        void ChooseService(IEnumerable<Host> hosts, List<ConfigSetting> config);
    }
}