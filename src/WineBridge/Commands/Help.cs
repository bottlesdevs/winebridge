namespace WineBridge.Commands;

internal class Help : ICommand
{
    public void Execute(string[]? args)
    {
        Console.WriteLine("List of commands:");
        foreach (var command in Data.Commands)
        {
            Console.WriteLine(command.Key);
        }
    }
}