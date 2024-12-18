using System.Diagnostics;
using System.Runtime.InteropServices;

namespace YouTbDownloader.Infrastructure;

public class YtDlp : ISetupLibrary
{
    public async Task<string> DownloadAndSetup()
    {
        var downloadUrl = "https://github.com/yt-dlp/yt-dlp/release/latests/download/yt-dlp";
        var filename = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "yt-dlp.exe" : "yt-dlp";
        var targetPath = Path.Combine(Environment.CurrentDirectory, filename);

        if (File.Exists(targetPath)) 
            return targetPath;
        
        using var httpClient = new HttpClient();
        var response = await httpClient.GetAsync(downloadUrl);
        response.EnsureSuccessStatusCode();

        await using var fs = new FileStream(targetPath, FileMode.Create, FileAccess.Write);
        await response.Content.CopyToAsync(fs);
            
        GrantedPermission(targetPath);

        return targetPath;
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