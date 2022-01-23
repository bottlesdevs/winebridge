using System.Diagnostics;

namespace WineBridge.Commands;

internal class KillProc : ICommand
{
    public void Execute(string[]? args)
    {
        if (args is null)
        {
            Console.WriteLine("Error. Usage: WineBridge.exe killProc ProcessID");
            return;
        }
        if (args.Length == 0)
        {
            Console.WriteLine("Error. Usage: WineBridge.exe killProc ProcessID");
            return;
        }

        foreach (var pidString in args)
        {
            if (!int.TryParse(pidString, out int pid))
            {
                Console.WriteLine($"Error killing process {pidString}. Invalid format.");
                continue;
            }
            
            var processes = Process.GetProcesses();
            foreach (var p in processes)
            {
                if (p.Id != pid) continue;
                try
                {
                    p.Kill();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error killing process {pidString}: {ex.Message}");
                }
            }
        }
    }
}