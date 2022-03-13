using System.Reflection;

namespace WineBridge.Commands;

internal class Help : ICommand
{
    public void Execute(string[]? args)
    {
        var version = typeof(Program).Assembly
            .GetCustomAttribute<AssemblyFileVersionAttribute>()
            ?.Version;
        Console.WriteLine($"Version: {version}");
        Console.WriteLine("List of commands:");
        foreach (var command in Data.Commands)
        {
            Console.WriteLine(command.Key);
        }
    }
}