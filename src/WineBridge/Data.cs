using WineBridge.Commands;

namespace WineBridge;

public class Data
{
    internal static Dictionary<string, ICommand> Commands = new();

    internal Data()
    {
        Commands.Add("HELP", new Help());
        Commands.Add("GETPROCS", new GetRunningProcs());
        Commands.Add("KILLPROC", new KillProc());
        Commands.Add("KILLPROCBYNAME", new KillProcByName());
        Commands.Add("RUNEXE", new RunExe());
    }
}