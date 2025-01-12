using YouTbDownloader.Conversor.Entities;
using YouTbDownloader.Conversor.Entities.Interfaces;

namespace YouTbDownloader.Conversor.Application.Services;

public class YtDlpService: IYtDlpService
{
    private readonly string _pathYTdlp;
    private readonly ICommandExecuteService _commandExecuteService;

    public YtDlpService(string pathYTdlp, ICommandExecuteService commandExecuteService)
    {
        _pathYTdlp = pathYTdlp;
        _commandExecuteService = commandExecuteService;
    }
    
    public async Task<VideoInfo> GetVideoTitle(string urlVideo)
    {
        var command = $"{_pathYTdlp} --print title {urlVideo}";
        var (output, _, exitCode)  = await _commandExecuteService.RunCommand(command);

        if (exitCode != 0)
            throw new Exception("Falha ao obter informações do vídeo");
        
        return new VideoInfo(output);
    }
}