namespace YouTbDownloader.Conversor.Entities.Interfaces;

public interface IYtDlpService
{
    Task<VideoInfo> GetVideoTitle(string urlVideo);
}