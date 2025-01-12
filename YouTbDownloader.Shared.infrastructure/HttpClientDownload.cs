namespace YouTbDownloader.Shared.infrastructure;

public class HttpClientDownload
{
    public async Task DownloadFile(string urlDownload, string pathToSave)
    {
        using var httpClient = new HttpClient();
        var requestMessage = new HttpRequestMessage(HttpMethod.Get, urlDownload);
        var response = await httpClient.SendAsync(requestMessage);
        response.EnsureSuccessStatusCode();

        await using var fs = new FileStream(pathToSave, FileMode.Create, FileAccess.Write);
        await response.Content.CopyToAsync(fs);
    }
}