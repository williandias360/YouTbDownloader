using System.Diagnostics;
using System.Runtime.InteropServices;

namespace YouTbDownloader.application;

public class Application
{
    private readonly ISetupLibrary _setupLibrary;
    private readonly ICommandExecute _commandExecute;
    const string APP_NAME = "YouTbDownloader";

    public Application(ISetupLibrary setupLibrary, ICommandExecute commandExecute)
    {
        _setupLibrary = setupLibrary;
        _commandExecute = commandExecute;
    }

    public async void Run()
    {
        try
        {
            Console.WriteLine("Inicializando ambiente");
            var path = await _setupLibrary.DownloadAndSetup();
            var pathffmpeg = await _setupLibrary.DownloadFfMgeg();
            var videoUrl = "https://music.youtube.com/watch?v=lpB-BML1nMY&si=bo4BnO-6Z5tI4U57";
            var outputPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), APP_NAME);
            if (!Directory.Exists(outputPath))
                Directory.CreateDirectory(outputPath);
        
            Console.WriteLine("Baixando vídeo");
            var completePath = $"{outputPath}/teste.mp3";
            var command = $"{path} --ffmpeg-location {pathffmpeg} -x --audio-format mp3 -o \"{completePath}\" {videoUrl}";
            var (outuput, error, exitCode) = await _commandExecute.RunCommand(command);
            
            if(!string.IsNullOrEmpty(outuput))
                Console.WriteLine($"Saída: {outuput}");
            
            if(!string.IsNullOrEmpty(error))
                Console.WriteLine($"Error: {error}");

            if (exitCode != 0)
                throw new Exception($"O comando falhou com código de saída {exitCode}");
            
            Console.WriteLine($"Áudio salvo em: {outputPath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro: {ex.Message}");
        }
        
    }
}