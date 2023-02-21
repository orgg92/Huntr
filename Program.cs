// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.DependencyInjection;
using Radar;
using Radar.Services;
using Radar.Services.Interfaces;

internal class Program
{
    private static void Main(string[] args)
    {
        var serviceProvider = new ServiceCollection()
            .AddSingleton<IIPManipulationService, IPManipulationService>()
            .AddSingleton<INetworkScanner, NetworkScanner>()
            .AddSingleton<IHostToolsService, HostToolsService>()
            .AddSingleton<ILoggingService, LoggingService>()
            .BuildServiceProvider();
        
        var ipManipulationService = serviceProvider.GetService<IIPManipulationService>();
        var networkService = serviceProvider.GetService<INetworkScanner>();
        var toolsService = serviceProvider.GetService<IHostToolsService>();
        var loggingService = serviceProvider.GetService<ILoggingService>();

        var radar = new RadarScanner(networkService, toolsService, loggingService);
        radar.StartApp();
    }
}