namespace WineBridge;

public interface ICommand
{
    void Execute(string[]? args);
}