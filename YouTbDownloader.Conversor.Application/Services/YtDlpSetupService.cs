using System.Runtime.InteropServices;
using YouTbDownloader.Conversor.Entities.Interfaces;
using YouTbDownloader.Shared.infrastructure;

namespace YouTbDownloader.Conversor.Application.Services;

public class YtDlpSetupService : IYtDlpSetupService
{
    private readonly HttpClientDownload _httpClientDownload;
    private readonly ICommandExecuteService _commandExecuteService;

    public YtDlpSetupService(HttpClientDownload httpClientDownload, ICommandExecuteService commandExecuteService)
    {
        _httpClientDownload = httpClientDownload;
        _commandExecuteService = commandExecuteService;
    }

    public async Task<string> DownloadAndSetup()
    {
        const string urlYtdlp = "https://github.com/yt-dlp/yt-dlp/releases/latest/download/yt-dlp";
        var filename = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "yt-dlp.exe" : "yt-dlp";
        var targetPath = Path.Combine(Environment.CurrentDirectory, filename);

        if (File.Exists(targetPath))
        {
            await GrantedPermission(targetPath);
            return targetPath;
        }

        await _httpClientDownload.DownloadFile(urlYtdlp, targetPath);
        await GrantedPermission(targetPath);
        return targetPath;
    }

    public async Task<string> DownloadFfMgeg()
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        var ffmpegDir = Path.Combine(currentDirectory, "ffmpegDir");
        const string ffmpegUrl = "https://johnvansickle.com/ffmpeg/releases/ffmpeg-release-amd64-static.tar.xz";

        if (Directory.Exists(ffmpegDir))
            return ffmpegDir;

        var tmpFile = Path.Combine(ffmpegDir, "ffmpeg.tar.xz");
        Directory.CreateDirectory(ffmpegDir);

        if (!File.Exists(tmpFile))
            await _httpClientDownload.DownloadFile(ffmpegDir, tmpFile);
        
        var tmpExtractDir = Path.Combine(ffmpegDir, "tmp");
        Directory.CreateDirectory(tmpExtractDir);
        var tarCommand = $"tar -xf {tmpFile} -C {tmpExtractDir}";
        await _commandExecuteService.RunCommand(tarCommand);
        
        var extractedDir = Directory.GetDirectories(tmpExtractDir)[0];
        foreach (var file in Directory.GetFiles(extractedDir))
        {
            var destFile = Path.Combine(ffmpegDir, Path.GetFileName(file));
            File.Move(file, destFile, true);
        }
        
        Directory.Delete(tmpExtractDir, true);
        File.Delete(tmpFile);

        return ffmpegDir;
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