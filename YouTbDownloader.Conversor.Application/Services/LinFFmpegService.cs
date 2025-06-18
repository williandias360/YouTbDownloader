using YouTbDownloader.Conversor.Entities.Interfaces;

namespace YouTbDownloader.Conversor.Application.Services
{
    public class LinFFmpegService(ICommandExecute _commandExecute) : IFFmpeg
    {
        public string DownloadFfMpeg()
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var ffmpegDir = Path.Combine(currentDirectory, "ffmpegDir");
            const string ffmpegUrl = "https://johnvansickle.com/ffmpeg/releases/ffmpeg-release-amd64-static.tar.xz";

            if (Directory.Exists(ffmpegDir) && File.Exists($"{ffmpegDir}/ffmpeg"))
                return ffmpegDir;

            var tmpFile = Path.Combine(ffmpegDir, "ffmpeg.tar.xz");
            Directory.CreateDirectory(ffmpegDir);

            if (!File.Exists(tmpFile))
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
            var tarCommand = $"tar -xf {tmpFile} -C {tmpExtractDir}";
            _commandExecute.RunCommand(tarCommand);

            var extractedDir = Directory.GetDirectories(tmpExtractDir)[0];
            foreach (var file in Directory.GetFiles(extractedDir))
            {
                var destFile = Path.Combine(ffmpegDir, Path.GetFileName(file));
                File.Move(file, destFile, true);
            }

            Directory.Delete(tmpExtractDir, true);
            File.Delete(tmpFile);

            return ffmpegDir;
        }
    }
}
