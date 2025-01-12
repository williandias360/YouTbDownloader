namespace YouTbDownloader.Conversor.Entities.Interfaces;

public interface IYtDlpSetupService
{
    Task<string> DownloadAndSetup();
    Task<string> DownloadFfMgeg();
}