using Microsoft.Extensions.DependencyInjection;
using YouTbDownloader.application;
using YouTbDownloader.Infrastructure;

namespace YouTbDownloader.Config;

public class Ioc
{
    public static IServiceProvider RegistrarDependencias()
    {
        var serviceProvider = new ServiceCollection()
            .AddTransient<Application>()
            .AddTransient<IYtDlpService, YtDlp>()
            .AddTransient<ICommandExecute, CommandExecute>()
            .BuildServiceProvider();

        return serviceProvider;
    }
}