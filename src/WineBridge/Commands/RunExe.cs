using System.Diagnostics;

namespace WineBridge.Commands;

internal class RunExe : ICommand
{
    public void Execute(string[]? args)
    {
        if (args is null)
        {
            Console.WriteLine("Error. Usage: WineBridge.exe runExe fullPathToExe [arg1] [arg2] [arg3...]");
            return;
        }
        if (args.Length == 0)
        {
            Console.WriteLine("Error. Usage: WineBridge.exe runExe fullPathToExe [arg1] [arg2] [arg3...]");
            return;
        }
        
        var executable = args[0];
        var arguments = args.Skip(1).ToArray();
            
        var startInfo = new ProcessStartInfo
        {
            FileName = executable,
            Arguments = string.Join(" ", arguments)
        };

        Process.Start(startInfo);
    }
}