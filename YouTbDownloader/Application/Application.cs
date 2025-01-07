using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;
using YouTbDownloader.Config;

namespace YouTbDownloader.application;

public partial class Application
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
            var pathYtDlp = await _ytDlpService.DownloadAndSetup();
            var pathffmpeg = await _ytDlpService.DownloadFfMgeg();
            var outputPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), APP_NAME);

            if (!Directory.Exists(outputPath))
                Directory.CreateDirectory(outputPath);

            var listaUrls = new[]
            {
                "https://music.youtube.com/watch?v=uP0wu72r9y4&si=UFs_mmdRsT136YU1",
                "https://music.youtube.com/watch?v=LuSkCkeMGdI&si=ddWluejKjqT0jAnX",
                "https://music.youtube.com/watch?v=NDfNe0qh9w0&si=_esOlUbTn1MPQ08c",
                "https://music.youtube.com/watch?v=D7f49tv48vM&si=eUjfvjr_GF02UW64",
                "https://music.youtube.com/watch?v=LCnqiXLr1Mg&si=uj42ivRIrIfxtkSL",
                "https://music.youtube.com/watch?v=WHDluxybYwk&si=80toZS7m92a-bByB",
                "https://music.youtube.com/watch?v=y_q-LWqOHiU&si=Y54MnN9GT6cf74rH",
                "https://music.youtube.com/watch?v=fKlhVpoACCI&si=PMeQHdqTN2UgxP6J",
                "https://music.youtube.com/watch?v=3YgLiNdINCw&si=JHCSNzRaucFIOsYS",
                "https://music.youtube.com/watch?v=3qJ8gz9ZVR4&si=1-LrgCVr1TOO8J3I",
                "https://music.youtube.com/watch?v=UXpv_wYwgl8&si=7wPjSrTiUwu7OG8m",
                "https://music.youtube.com/watch?v=LDDVQASOd9I&si=UH2HlPSAk7tJgTgk",
                "https://music.youtube.com/watch?v=inSoW5szMrA&si=eVUIh_PUcgKkBfEY",
                "https://music.youtube.com/watch?v=kU9VuSH1Jn0&si=Ozg5boEYgv5cB_8H",
                "https://music.youtube.com/watch?v=QZYnC1H_ITM&si=7b5czUkQRiTrE_Rx",
                "https://music.youtube.com/watch?v=0-n-OBSl7S0&si=-hHwkvEvKPpklAlU",
                "https://music.youtube.com/watch?v=GKa04Ee1ewM&si=hx555iLmxSRSlBWP",
                "https://music.youtube.com/watch?v=u-5XZbls6DA&si=XqrAqmctJRRl811U",
                "https://music.youtube.com/watch?v=gZHLBYz4HNM&si=59aM9N746NIel8wn",
                "https://music.youtube.com/watch?v=54KvRnRET4k&si=dcwQjC82Wu-tDg98",
                "https://music.youtube.com/watch?v=54KvRnRET4k&si=cEzVA-wsTpf5DTtv",
                "https://music.youtube.com/watch?v=7DFIq84Da8w&si=fGTS4K6WtYBPdOIL",
                "https://music.youtube.com/watch?v=jdY8iapi2kY&si=Z6v5Z0ggreS0kxme",
                "https://music.youtube.com/watch?v=f8_8vnTVZKs&si=cQ2_V33W7hzWn7X-",
                "https://music.youtube.com/watch?v=CBCFQc0ck_8&si=YykJPOXCPC_0NJIf",
                "https://music.youtube.com/watch?v=2XiMb5s15hk&si=n1zcajiREzzkLwL3",
                "https://music.youtube.com/watch?v=mXk3Mou6494&si=I01Vbnk_MmHOY-4O",
                "https://music.youtube.com/watch?v=gmV3p1Wq7bg&si=07ce8Amllzdi9EKw",
                "https://music.youtube.com/watch?v=XrqaPNAM86s&si=Q3okTbv9fLuAqZV3",
                "https://music.youtube.com/watch?v=SX0095lIykI&si=bffBeWUoP5plJnUo",
                "https://music.youtube.com/watch?v=ODXYM7KQBR4&si=xHHbv1ntD8jdMsTb",
                "https://music.youtube.com/watch?v=RiLoSO6N44M&si=Muaac0acC-CcXCAq",
                "https://music.youtube.com/watch?v=KHJukRJXXyc&si=2aYiHhWbFivbKVF4",
                "https://music.youtube.com/watch?v=IVQML7vs6Cc&si=1nL2PFpFKjK-gAU_",
                "https://music.youtube.com/watch?v=bJISJ42KEqQ&si=bFXzGJbHgOWbs-kG",
                "https://music.youtube.com/watch?v=g6jRVJ3Nsg0&si=3d8jprNs7mD4VQYz",
                "https://music.youtube.com/watch?v=whWkgm5I2X0&si=ORNgVHWPItS8fx2e",
                "https://music.youtube.com/watch?v=gTBNYllA6I4&si=R66cuvwhy8H5i7E-",
                "https://music.youtube.com/watch?v=fyU-IoOL5Fw&si=K03YtatFvR7Mb68z",
                "https://music.youtube.com/watch?v=miG2HCG7hcQ&si=z3tcUP5m7gm1Q223",
                "https://music.youtube.com/watch?v=-qIvrotkGbI&si=lhar4jVp4gGd3P5I",
                "https://music.youtube.com/watch?v=69V7g-iXhjk&si=94IytBLpamsS8cHh",
                "https://music.youtube.com/watch?v=0SclapVyTAM&si=b0sUl8XD34ygeoKN",
                "https://music.youtube.com/watch?v=T4kuJP7BeaA&si=2e9Y0fPkP_D8nXKT",
                "https://music.youtube.com/watch?v=mz_O5S-9gfs&si=o7fA-GWPm9Ytxsl-",
                "https://music.youtube.com/watch?v=n6ruSMV9yzs&si=1_ZbwjSonOZbtnJ1",
                "https://music.youtube.com/watch?v=T3XpldCB7O8&si=HBmtlSS7qED-5Wcu",
                "https://music.youtube.com/watch?v=GFbhncHHB-Y&si=_S-NLietamK4TTIm",
                "https://music.youtube.com/watch?v=rf95ksS3JCA&si=kF51FPRO3EyvNzhc",
                "https://music.youtube.com/watch?v=ZiRfNKkUvds&si=NQ0jmwL2oNsJoazd",
                "https://music.youtube.com/watch?v=FfngPM2MIU4&si=ycTBC1F2okMJ2DhC",
                "https://music.youtube.com/watch?v=kEz5bJuReac&si=uNsFfwSkvuXTFyxh",
                "https://music.youtube.com/watch?v=TJ8EqKXANBI&si=J5o1SNDEqBGs4mW-",
                "https://music.youtube.com/watch?v=0F9tzxhLS_8&si=-fjP6Cv6UI4zTcy3",
                "https://music.youtube.com/watch?v=fEH5-Iikcd4&si=FP7g4jcJSJUI38g-",
                "https://music.youtube.com/watch?v=R_hyZ6Z0VNQ&si=F2cNiC8W3zaA5yDA",
                "https://music.youtube.com/watch?v=CkCErOkKtqs&si=W9Eg5GtiT4XApqGs",
                "https://music.youtube.com/watch?v=-XlcMU66lzw&si=3fteI5G2S_ay5S7M",
                "https://music.youtube.com/watch?v=q4zpp00SYQI&si=flO3MZgduHkULESR",//aqui
                "https://music.youtube.com/watch?v=A-5Ri8aCfGE&si=8UkOTwdEi3OZJVtG",
                "https://music.youtube.com/watch?v=CC-6eL-fYbQ&si=M3mc5l7LbAyROLWL",
                "https://music.youtube.com/watch?v=weHDtD44jjY&si=FnNyMaTYeD9KHUME",
                "https://music.youtube.com/watch?v=Nnnfzx6dR1E&si=Tn7aBfoerg4mqeCj",
            };

            var listaTasks = new List<Task>();
            var semaphore = new SemaphoreSlim(10);
            foreach (var videoUrl in listaUrls)
            {
                var task = Task.Run(async () =>
                {
                    try
                    {
                        await semaphore.WaitAsync();
                        Console.WriteLine("Obtendo informações do vídeo");
                        
                        var titulo = await _ytDlpService.GetVideoTitle(pathYtDlp, videoUrl);
                        Console.WriteLine($"Baixando vídeo {titulo}");

                        var guid = Guid.NewGuid();
                        var completePath = $"{outputPath}/{guid}.mp3";
                        var command =
                            $"{pathYtDlp} --ffmpeg-location {pathffmpeg} -x --audio-format mp3 -o \"{completePath}\" {videoUrl}";
                        var (_, error, exitCode) = await _commandExecute.RunCommand(command);

                        if (exitCode != 0)
                        {
                            if (!string.IsNullOrEmpty(error))
                                Console.WriteLine($"Error: {error}");

                            Console.WriteLine($"O comando falhou com código de saída {exitCode}");
                            return;
                        }
                        
                        var newName = Path.Combine(outputPath, $"{NormalizeString(titulo)}.mp3");
                        var commandRename = $"mv {completePath} '{newName}'";
                        await _commandExecute.RunCommand(commandRename);

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