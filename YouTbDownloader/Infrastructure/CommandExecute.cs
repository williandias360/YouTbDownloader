using System.Diagnostics;
using System.Runtime.InteropServices;
using YouTbDownloader.Conversor.Entities.Interfaces;

namespace YouTbDownloader.Infrastructure;

public class CommandExecute : ICommandExecute
{
    public (string output, string error, int exitCode) RunCommand(string command)
    {
        using var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "cmd" : "bash",
                Arguments = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? $"/c {command}" : $"-c \"{command}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                StandardOutputEncoding = System.Text.Encoding.Latin1,
                StandardErrorEncoding = System.Text.Encoding.Latin1,
            }
        };

        process.Start();
        var output = process.StandardOutput.ReadToEnd();
        var error = process.StandardError.ReadToEnd();
        process.WaitForExit();

        return (output, error, process.ExitCode);
    }
}