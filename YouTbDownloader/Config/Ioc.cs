using Microsoft.Extensions.DependencyInjection;
using System.Runtime.InteropServices;
using YouTbDownloader.application;
using YouTbDownloader.Conversor.Application.Services;
using YouTbDownloader.Conversor.Entities.Interfaces;
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
            .AddTransient<IFFmpeg>((provider) =>
            {
                var commandExecute = provider.GetRequiredService<ICommandExecute>();
                
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    return new WinFFmpepService(commandExecute);

                return new LinFFmpegService(commandExecute);
            })
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