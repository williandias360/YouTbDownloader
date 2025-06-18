using SevenZip;
using YouTbDownloader.Conversor.Entities.Interfaces;

namespace YouTbDownloader.Conversor.Application.Services
{
    public class WinFFmpepService(ICommandExecute commandExecute) : IFFmpeg
    {
        public string DownloadFfMpeg()
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var ffmpegDir = Path.Combine(currentDirectory, "ffmpegDir");
            const string ffmpegUrl = "https://www.gyan.dev/ffmpeg/builds/ffmpeg-git-essentials.7z";

            if(Directory.Exists(ffmpegDir) && File.Exists(Path.Combine(ffmpegDir, "ffmpeg.exe")))
                return ffmpegDir;

            var tmpFile = Path.Combine(ffmpegDir, "ffmpeg.7z");
            Directory.CreateDirectory(ffmpegDir);

            if(!File.Exists(tmpFile))
            {
                using var httpClient = new HttpClient();
                var requestMessage = new HttpRequestMessage(HttpMethod.Get, ffmpegUrl);
                var response = httpClient.Send(requestMessage);
                response.EnsureSuccessStatusCode();
                using var fs = new FileStream(tmpFile, FileMode.Create, FileAccess.Write);
                response.Content.CopyTo(fs, null, default);
            }

            var tmpExtractDir = Path.Combine(ffmpegDir, "tmp");
            Directory.CreateDirectory(tmpExtractDir);
            SevenZipBase.SetLibraryPath(Path.Combine(currentDirectory, "7z.dll"));
            using var extractor = new SevenZipExtractor(tmpFile);
            extractor.ExtractArchive(tmpExtractDir);

            var extractedDir = Directory.GetDirectories(tmpExtractDir)[0];
            foreach (var directory in Directory.GetDirectories(extractedDir))
            {
                if(directory.EndsWith("bin", StringComparison.CurrentCultureIgnoreCase))
                {
                    var files = Directory.GetFiles(directory);
                    foreach (var file in files)
                    {
                        var destFile = Path.Combine(ffmpegDir, Path.GetFileName(file));
                        File.Move(file, destFile, true);
                    }
                }
            }

            Directory.Delete(tmpExtractDir, true);
            File.Delete(tmpFile);

            return ffmpegDir;
        }
    }
}
