using System.Diagnostics;
using System.Runtime.InteropServices;

namespace YouTbDownloader.Infrastructure;

public class YtDlp : ISetupLibrary
{
    private readonly ICommandExecute _commandExecute;

    public YtDlp(ICommandExecute commandExecute)
    {
        _commandExecute = commandExecute;
    }

    public async Task<string> DownloadAndSetup()
    {
        var downloadUrl = "https://github.com/yt-dlp/yt-dlp/releases/latest/download/yt-dlp";
        var filename = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "yt-dlp.exe" : "yt-dlp";
        var targetPath = Path.Combine(Environment.CurrentDirectory, filename);

        if (File.Exists(targetPath))
        {
            GrantedPermission(targetPath);
            return targetPath;
        }
        
        using var httpClient = new HttpClient();
        var requestMessage = new HttpRequestMessage(HttpMethod.Get, downloadUrl);
        var response = httpClient.Send(requestMessage);
        response.EnsureSuccessStatusCode();

        await using var fs = new FileStream(targetPath, FileMode.Create, FileAccess.Write);
        await response.Content.CopyToAsync(fs);
            
        GrantedPermission(targetPath);

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
        {
            using var httpClient = new HttpClient();
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, ffmpegUrl);
            var response = httpClient.Send(requestMessage);
            response.EnsureSuccessStatusCode();

            try
            {
                await using var fs = new FileStream(tmpFile, FileMode.Create, FileAccess.Write);
                await response.Content.CopyToAsync(fs);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
        }

        var tmpExtractDir = Path.Combine(ffmpegDir, "tmp");
        Directory.CreateDirectory(tmpExtractDir);
        var tarCommand = $"tar -xf {tmpFile} -C {tmpExtractDir}";
        await _commandExecute.RunCommand(tarCommand);

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

    private void GrantedPermission(string targetPath)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) 
            return;
        
        var chmod = new Process
        {
            StartInfo = new ProcessStartInfo()
            {
                FileName = "chmod",
                Arguments = $"a+rx {targetPath}",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        chmod.Start();
        chmod.WaitForExit();
    }
}