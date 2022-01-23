using System.Diagnostics;

namespace WineBridge.Commands;

internal class KillProcByName : ICommand
{
    public void Execute(string[]? args)
    {
        if (args is null)
        {
            Console.WriteLine("Error. Usage: WineBridge.exe killProcByName ProcessName");
            return;
        }
        if (args.Length == 0)
        {
            Console.WriteLine("Error. Usage: WineBridge.exe killProcByName ProcessName");
            return;
        }

        foreach (var processName in args)
        {
            var processes = Process.GetProcesses();
            foreach (var p in processes)
            {
                if (p.ProcessName != processName) continue;
                try
                {
                    p.Kill();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error killing process {processName}: {ex.Message}");
                }
            }
        }
    }
}