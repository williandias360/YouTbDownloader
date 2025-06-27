using System.Runtime.InteropServices;
using YouTbDownloader.Conversor.Entities.Interfaces;
using YouTbDownloader.Shared.infrastructure;

namespace YouTbDownloader.Conversor.Application.Services;

public class YtDlpSetupService : IYtDlpSetupService
{
    private readonly HttpClientDownload _httpClientDownload;
    private readonly ICommandExecuteService _commandExecuteService;
    private readonly IFFmpeg _ffmpeg;

    public YtDlpSetupService(HttpClientDownload httpClientDownload, ICommandExecuteService commandExecuteService, IFFmpeg ffmpeg)
    {
        _httpClientDownload = httpClientDownload;
        _commandExecuteService = commandExecuteService;
        _ffmpeg = ffmpeg;
    }

    private async Task<string> DownloadAndSetup()
    {
        var plataforma = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "yt-dlp.exe" : "yt-dlp";
        var downloadUrl = $"https://github.com/yt-dlp/yt-dlp/releases/latest/download/{plataforma}";
        var filename = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "yt-dlp.exe" : "yt-dlp";
        var targetPath = Path.Combine(Environment.CurrentDirectory, filename);

        if (File.Exists(targetPath))
        {
            await GrantedPermission(targetPath);
            return targetPath;
        }

        await _httpClientDownload.DownloadFile(downloadUrl, targetPath);
        await GrantedPermission(targetPath);

        return targetPath;
    }

    public string DownloadFfMgeg()
    {
        return _ffmpeg.DownloadFfMpeg();
    }

    #region Privados

    private async Task GrantedPermission(string targetPath)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return;

        var command = $"chmod a+rx {targetPath}";
        await _commandExecuteService.RunCommand(command);
    }

    #endregion
}