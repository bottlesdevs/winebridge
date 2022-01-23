using WineBridge;

if (args.Length == 0)
{
    Console.WriteLine("Usage: WineBridge.exe <event> <args>");
    Console.WriteLine("Use: WineBridge.exe help to get a list of commands.");
    return;
}

_ = new Data();

var command = args[0];
if(!Data.Commands.TryGetValue(command.ToUpper(), out ICommand parsedCommand))
{
    Console.WriteLine("Invalid command!");
    Console.WriteLine("Use: WineBridge.exe help to get a list of commands.");
    return;
}
parsedCommand.Execute(args.Skip(1).ToArray());