namespace YouTbDownloader;

public interface ICommandExecute
{
    Task<(string output, string error, int exitCode)> RunCommand(string command);
}