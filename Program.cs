// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.DependencyInjection;
using Radar;
using Radar.Services;
using Radar.Services.Interfaces;
using System.Reflection;
using MediatR;

internal class Program
{
    private static void Main(string[] args)
    {
        var serviceProvider = new ServiceCollection()
            .AddMediatR(Assembly.GetExecutingAssembly())
            .AddSingleton<IIPManipulationService, IPManipulationService>()
            .AddSingleton<INetworkScanner, NetworkScanner>()
            .AddSingleton<IHostToolsService, HostToolsService>()
            .AddSingleton<ILoggingService, LoggingService>()
            //.AddSingleton<ISSHService, SSHService>()
            .BuildServiceProvider();

        var ipManipulationService = serviceProvider.GetService<IIPManipulationService>();
        var networkService = serviceProvider.GetService<INetworkScanner>();
        var toolsService = serviceProvider.GetService<IHostToolsService>();
        var loggingService = serviceProvider.GetService<ILoggingService>();
        var sshService = serviceProvider.GetService<ISSHService>();

        var radar = new RadarScanner(networkService, toolsService, loggingService);
        radar.StartApp();
    }
}