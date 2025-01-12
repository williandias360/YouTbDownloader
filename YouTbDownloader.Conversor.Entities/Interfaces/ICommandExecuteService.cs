namespace YouTbDownloader.Conversor.Entities.Interfaces;

public interface ICommandExecuteService
{
    Task<(string output, string error, int exitCode)> RunCommand(string command);
}