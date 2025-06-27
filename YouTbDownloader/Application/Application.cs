using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using YouTbDownloader.Conversor.Entities.Interfaces;

namespace YouTbDownloader.application;

public partial class Application
{
    private readonly IYtDlpService _ytDlpService;
    private readonly ICommandExecute _commandExecute;
    private readonly IYtDlpSetupService _ytDlpSetupService;
    const string APP_NAME = "YouTbDownloader";

    public Application(IYtDlpService ytDlpService, ICommandExecute commandExecute, IYtDlpSetupService ytDlpSetupService)
    {
        _ytDlpService = ytDlpService;
        _commandExecute = commandExecute;
        _ytDlpSetupService = ytDlpSetupService;
    }

    public void Run()
    {
        try
        {
            Console.WriteLine("Inicializando ambiente");
            var pathYtDlp = _ytDlpService.DownloadAndSetup();
            var pathffmpeg = _ytDlpSetupService.DownloadFfMgeg();
            var outputPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), APP_NAME);

            if (!Directory.Exists(outputPath))
                Directory.CreateDirectory(outputPath);

            var listaUrls = new[]
            {
                "https://youtu.be/artng5iskMw?si=2NbHWeV3HbW75Dhy",
                "https://youtu.be/I_e5OEfD2jY?si=IH9Z2PdfycNgnNga",
                "https://youtu.be/vpp-DP1JTLk?si=NzA7-BYuLVwTe3-N",
                "https://youtu.be/Xy6haZelsn0?si=YyPr3Ag-6XX6Zdig",
            };

            var listaTasks = new List<Task>();
            var semaphore = new SemaphoreSlim(1);
            foreach (var videoUrl in listaUrls)
            {
                var task = Task.Run(async () =>
                {
                    try
                    {
                        await semaphore.WaitAsync();
                        Console.WriteLine("Obtendo informações do vídeo");
                        
                        var titulo = _ytDlpService.GetVideoTitle(pathYtDlp, videoUrl);
                        Console.WriteLine($"Baixando vídeo {titulo}");

                        var guid = Guid.NewGuid();
                        var completePath = $"{outputPath}/{guid}.mp3";
                        var command = $"{pathYtDlp} --ffmpeg-location {pathffmpeg} -x --audio-format mp3 -o \"{completePath}\" \"{videoUrl}\"";
                        var (_, error, exitCode) = _commandExecute.RunCommand(command);

                        if (exitCode != 0)
                        {
                            if (!string.IsNullOrEmpty(error))
                                Console.WriteLine($"Error: {error}");

                            Console.WriteLine($"O comando falhou com código de saída {exitCode}");
                            return;
                        }
                        
                        var newName = Path.Combine(outputPath, $"{NormalizeString(titulo)}.mp3");
                        File.Move(completePath, newName, true);

                        Console.WriteLine($"Áudio salvo em: {outputPath}");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        throw;
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                });

                listaTasks.Add(task);
            }

            Task.WaitAll(listaTasks.ToArray());
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro: {ex.Message}");
        }
    }

    private string NormalizeString(string texto)
    {
        var sanitized = MyRegex().Replace(texto, " ");
        var normalized = sanitized.Normalize(NormalizationForm.FormD);
        var result = new StringBuilder();

        foreach (var letter in normalized.Where(letter => CharUnicodeInfo.GetUnicodeCategory(letter) != UnicodeCategory.NonSpacingMark))
        {
            result.Append(letter);
        }

        return result.ToString().Normalize(NormalizationForm.FormC);
    }

    [GeneratedRegex(@"[\\\""'\|/]")]
    private static partial Regex MyRegex();
}