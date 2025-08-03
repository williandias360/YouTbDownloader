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
                "https://music.youtube.com/watch?v=R73PZgUFTRo&si=zalZay7_3uUqNMYF",
                "https://music.youtube.com/watch?v=7AArNMYR1wc&si=teadRCcq_Uz4iOkC",
                "https://music.youtube.com/watch?v=V87_8N7bFfQ&si=R_5xyKdqCDgq-2Ht",
                "https://music.youtube.com/watch?v=tQ3npz_yX3M&si=iUqI-hJTM4zXW2yW",
                "https://music.youtube.com/watch?v=p3T5I0nLa70&si=MagaB5fbo0kTY5Pk",
                "https://music.youtube.com/watch?v=JfOaZDepXQQ&si=CLCJVyEYZTT3C5lX",
                "https://music.youtube.com/watch?v=jhsmxQdxCp4&si=5477hbiloq0YA08w",
                "https://music.youtube.com/watch?v=l3G3dxNkPSo&si=ylUptbGlOXxcNwP0",
                "https://music.youtube.com/watch?v=rGAdj9TdnAk&si=gaOFoZ4Tk13oE6zz",
                "https://music.youtube.com/watch?v=W2kwuDjcd50&si=t7xmSg07kxHDZtW0",
                "https://music.youtube.com/watch?v=acZQMWBInFw&si=_T75NWK8VV7yVvgT",
                "https://music.youtube.com/watch?v=7yOSOFn4R2w&si=e7RiZNd3sdilNwOw",
                "https://music.youtube.com/watch?v=QRZuBhhrsNs&si=VgWa79eeMEIdpoyH",
                "https://music.youtube.com/watch?v=oHG47Ggzn5A&si=U5Fms2aQB-DaGqMj",
                "https://music.youtube.com/watch?v=nAsYjyuD7ck&si=j_0-YES3sA0mM-9j",
                "https://music.youtube.com/watch?v=O2uiLD0-k8I&si=j6skdeYOKtcY8Hm7",
                "https://music.youtube.com/watch?v=2G8GHk2QWEk&si=-MapELSUWVE2-2fd",
                "https://music.youtube.com/watch?v=1umZOUBq56Q&si=zhy9cUws3xARDUJI",
                "https://music.youtube.com/watch?v=jSYxRrYJHS0&si=3fkPMco7dqTp0Lhf",
                "https://music.youtube.com/watch?v=xSh4aq7loPg&si=6ZiYqmF0splEIJDx",
                "https://music.youtube.com/watch?v=FXA31VVNNkI&si=4hMFFoNTlKHuZ0wP",
                "https://music.youtube.com/watch?v=7if6uroREk8&si=nAATnsJLJnJTVenw",
                "https://music.youtube.com/watch?v=3LcCpixu990&si=_OwQeq0VJbwyER3D",
                "https://music.youtube.com/watch?v=4insPVOTwTk&si=T5ytYYY8Ny1ZDUKb",
                "https://music.youtube.com/watch?v=EHx5x2sa4cc&si=WeeMmMmSOwzmOKIn",
                "https://music.youtube.com/watch?v=gCCtCp_tq5U&si=Gu6WiwJ45RMs6mMy",
                "https://music.youtube.com/watch?v=zdv_mdd0YSk&si=8FSkaq1GlFqA8866",
                "https://music.youtube.com/watch?v=CIqje2dQna8&si=nx1WviCV95X9xQaZ",
                "https://music.youtube.com/watch?v=gDkBpcsgCD8&si=pCKoTv1hJ758gH4e",
                "https://music.youtube.com/watch?v=uSC7vfnOWJg&si=uPuHIiXzhrggwWBd",
                "https://music.youtube.com/watch?v=fdoL-6wfGHg&si=LCfq7DBL6NRGlp7C",
                "https://music.youtube.com/watch?v=E3FAKlh1pS8&si=-9MCE8yV5fABmf2G",
                "https://music.youtube.com/watch?v=_NKOnCwP-Gw&si=oKr9_ePPLTFxSch7",
                "https://music.youtube.com/watch?v=Tr5AjI453yY&si=9z4Qud2MmjYrN1V2",
                "https://music.youtube.com/watch?v=jefWE1uPbuY&si=EOtAyKXpj0P03V64",
                "https://music.youtube.com/watch?v=tXNw6nqulJk&si=QMTSR2sfV7jLFjku",
                "https://music.youtube.com/watch?v=o52CYG6ux8w&si=B6DTIvRk1RXyP-Ms",
                "https://music.youtube.com/watch?v=bAnwj8LU0fY&si=s3PC3MtyCqvRoIvZ",
                "https://music.youtube.com/watch?v=FaKA_NHbGmc&si=BVirxjUUtJb4Ys-X",
                "https://music.youtube.com/watch?v=RUDlJpQvvsg&si=MSh0O2aCU_wQ2uTp",
                "https://music.youtube.com/watch?v=omZQlFn62yg&si=QUpPBnZNKwBRFJOQ",
                "https://music.youtube.com/watch?v=6KmaAkv7Wb8&si=EhS_yw5qYMFAR7iG",
                "https://music.youtube.com/watch?v=254JUnL-Dgg&si=jDTtAyy1PT-Xrcr4",
                "https://music.youtube.com/watch?v=aKJpgGUnEj8&si=qUL0XvzebTr3e6Pn",
                "https://music.youtube.com/watch?v=W16G42tej8M&si=y8g1OY6C2bfwkuIl",
                "https://music.youtube.com/watch?v=XsTbiqvgI3A&si=_bLwBUCuKiBj6Qyj",
                "https://music.youtube.com/watch?v=3ca-x-RUIWw&si=4hqZNjkO90Z9f0he",
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