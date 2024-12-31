using System.Diagnostics;
using System.Runtime.InteropServices;

namespace YouTbDownloader.application;

public class Application
{
    private readonly IYtDlpService _ytDlpService;
    private readonly ICommandExecute _commandExecute;
    const string APP_NAME = "YouTbDownloader";

    public Application(IYtDlpService ytDlpService, ICommandExecute commandExecute)
    {
        _ytDlpService = ytDlpService;
        _commandExecute = commandExecute;
    }

    public async void Run()
    {
        try
        {
            Console.WriteLine("Inicializando ambiente");
            var path = await _ytDlpService.DownloadAndSetup();
            var pathffmpeg = await _ytDlpService.DownloadFfMgeg();
            var outputPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), APP_NAME);
            if (!Directory.Exists(outputPath))
                Directory.CreateDirectory(outputPath);

            var videoUrl = "https://music.youtube.com/watch?v=lpB-BML1nMY&si=bo4BnO-6Z5tI4U57";
            Console.WriteLine("Obtendo informações do vídeo");
            var titulo = await _ytDlpService.GetVideoTitle(videoUrl);
            titulo = titulo.Replace(" ", "_");
            Console.WriteLine("Baixando vídeo");
            var completePath = $"{outputPath}/{titulo}.mp3";
            var command = $"{path} --ffmpeg-location {pathffmpeg} -x --audio-format mp3 -o \"{completePath}\" {videoUrl}";
            var (outuput, error, exitCode) = await _commandExecute.RunCommand(command);
            
            if(!string.IsNullOrEmpty(outuput))
                Console.WriteLine($"Saída: {outuput}");
            
            if(!string.IsNullOrEmpty(error))
                Console.WriteLine($"Error: {error}");

            if (exitCode != 0)
                throw new Exception($"O comando falhou com código de saída {exitCode}");

            var commandRename = $"mv {completePath} '{completePath.Replace("_", " ")}'";
            await _commandExecute.RunCommand(commandRename);
            
            Console.WriteLine($"Áudio salvo em: {outputPath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro: {ex.Message}");
        }
        
    }
}