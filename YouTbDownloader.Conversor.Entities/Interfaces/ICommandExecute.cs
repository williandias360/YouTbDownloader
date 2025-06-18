namespace YouTbDownloader.Conversor.Entities.Interfaces;

public interface ICommandExecute
{
    (string output, string error, int exitCode) RunCommand(string command);
}