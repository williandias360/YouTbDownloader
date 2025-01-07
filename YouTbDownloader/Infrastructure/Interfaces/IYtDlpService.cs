namespace YouTbDownloader;

public interface IYtDlpService
{
    Task<string> DownloadAndSetup();
    Task<string> DownloadFfMgeg();
    Task<string> GetVideoTitle(string pathYtDlp, string urlVideo);
}