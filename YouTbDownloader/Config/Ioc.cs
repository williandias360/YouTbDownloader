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

    public static IServiceProvider Create()
    {
        var serviceProvider = new ServiceCollection()
            .AddScoped<IYtDlpService, YtDlp>()
            .AddScoped<ICommandExecute, CommandExecute>()
            .BuildServiceProvider();

        return serviceProvider;
    }
}