using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace WineBridge.Commands;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
public class GetRunningProcs : ICommand
{
    public void Execute(string[]? args)
    {
        var processes = Process.GetProcesses();
        var output = string.Empty;

        foreach (var p in processes)
        {
            output += $"{p.ProcessName}|{p.Id}|{p.Threads.Count}\n";
        }

        Console.WriteLine(output);
    }
}