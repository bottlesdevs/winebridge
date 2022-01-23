using System.Diagnostics;

namespace WineBridge.Commands;

public class GetRunningProcs : ICommand
{
    public void Execute(string[]? args)
    {
        var processes = Process.GetProcesses();
        var output = string.Empty;

        foreach (var p in processes)
        {
            var pid = p.Id.ToString("X");
            output += $"{p.ProcessName} | {pid}\n";
        }

        Console.WriteLine(output);
    }
}