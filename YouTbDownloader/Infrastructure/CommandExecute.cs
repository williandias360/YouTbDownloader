using System.Diagnostics;
using System.Runtime.InteropServices;

namespace YouTbDownloader.Infrastructure;

public class CommandExecute : ICommandExecute
{
    public async Task<(string output, string error, int exitCode)> RunCommand(string command)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "cmd" : "bash",
                Arguments = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? $"/c {command}" : $"-c \"{command}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        process.Start();
        var output =  await process.StandardOutput.ReadToEndAsync();
        var error =  await process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync();
        return (output, error, process.ExitCode);
    }
}