// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.DependencyInjection;
using Radar;
using Radar.Services;
using Radar.Services.Interfaces;

internal class Program
{
    private async static Task Main(string[] args)
    {
        //setup our DI
        var serviceProvider = new ServiceCollection()
            .AddSingleton<IIPManipulationService, IPManipulationService>()
            .AddSingleton<INetworkScanner, NetworkScanner>()
            .AddSingleton<IHostToolsService, HostToolsService>()
            .BuildServiceProvider();

        var ipService = serviceProvider.GetService<IIPManipulationService>();
        var networkService = serviceProvider.GetService<INetworkScanner>();
        var toolsService = serviceProvider.GetService<IHostToolsService>();

        var radar = new RadarScanner(ipService, networkService, toolsService);
        radar.StartApp();
    }
}