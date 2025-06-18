namespace YouTbDownloader;

public interface IYtDlpService
{
    string DownloadAndSetup();
    string DownloadFfMgeg();
    string GetVideoTitle(string pathYtDlp, string urlVideo);
}