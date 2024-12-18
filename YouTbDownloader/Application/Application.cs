using System.Diagnostics;
using System.Runtime.InteropServices;

namespace YouTbDownloader.application;

public class Application
{
    private readonly ISetupLibrary _setupLibrary;
    const string APP_NAME = "YouTbDownloader";

    public Application(ISetupLibrary setupLibrary)
    {
        _setupLibrary = setupLibrary;
    }

    public async void Run()
    {
        try
        {
            Console.WriteLine("Inicializando ambiente");
            var path = await _setupLibrary.DownloadAndSetup();
            var videoUrl = "https://music.youtube.com/watch?v=lpB-BML1nMY&si=bo4BnO-6Z5tI4U57";
            var outputPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), APP_NAME);
        
            Console.WriteLine("Baixando vídeo");
            await RunCommand($"{path} - x --audio-format mp3 -o \"{outputPath}\" {videoUrl}");
            Console.WriteLine($"Áudio salvo em: {outputPath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro: {ex.Message}");
        }
        
    }

    private async Task RunCommand(string command)
    {
        Console.WriteLine($"Executando comando {command}");
        var process = new Process()
        {
            StartInfo = new ProcessStartInfo()
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
        var output = await process.StandardOutput.ReadToEndAsync();
        var error = await process.StandardError.ReadToEndAsync();
        process.WaitForExit();
        
        if(!string.IsNullOrEmpty(output))
            Console.WriteLine($"Saída: {output}");
        
        if(!string.IsNullOrEmpty(error))
            Console.WriteLine($"Erro: {error}");

        if (process.ExitCode != 0)
            throw new Exception($"O comando falhou com código de saída {process.ExitCode}");

    }
}