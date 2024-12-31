namespace YouTbDownloader;

public interface ISetupLibrary
{
    Task<string> DownloadAndSetup();
    Task<string> DownloadFfMgeg();
}