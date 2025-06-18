using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.Json;
using YouTbDownloader.Conversor.Entities.Interfaces;

namespace YouTbDownloader.Infrastructure;

public class YtDlp(ICommandExecute _commandExecute, IFFmpeg _ffMpeg) : IYtDlpService
{
    private string? _ytDlpPath;

    public string DownloadAndSetup()
    {
        var plataforma = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "yt-dlp.exe" : "yt-dlp";
        var downloadUrl = $"https://github.com/yt-dlp/yt-dlp/releases/latest/download/{plataforma}";
        var filename = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "yt-dlp.exe" : "yt-dlp";
        var targetPath = Path.Combine(Environment.CurrentDirectory, filename);

        if (File.Exists(targetPath))
        {
            GrantedPermission(targetPath);
            _ytDlpPath = targetPath;
            return targetPath;
        }
        
        using var httpClient = new HttpClient();
        var requestMessage = new HttpRequestMessage(HttpMethod.Get, downloadUrl);
        var response = httpClient.Send(requestMessage);
        response.EnsureSuccessStatusCode();

        using var fs = new FileStream(targetPath, FileMode.Create, FileAccess.Write);
        response.Content.CopyTo(fs, null, default);
            
        GrantedPermission(targetPath);

        return targetPath;
    }

    public string DownloadFfMgeg()
    {
        return _ffMpeg.DownloadFfMpeg();
    }

    public string GetVideoTitle(string pathYtDlp, string urlVideo)
    {
        // Adiciona aspas duplas ao redor da URL do vídeo
        var quotedUrl = $"\"{urlVideo}\"";
        var command = @$"{pathYtDlp} --print title {quotedUrl}";
        var (output, _, exitCode) = _commandExecute.RunCommand(command);

        if (exitCode != 0)
            throw new Exception("Falha ao obter informações do vídeo");

        if (!string.IsNullOrEmpty(output)) 
            return output.Replace("\n", "");
        
        command = $"{_ytDlpPath} --dump-json {urlVideo}";
        (output, _, _) = _commandExecute.RunCommand(command);

        return output;
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