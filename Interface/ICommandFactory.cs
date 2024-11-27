public interface ICommandFactory
{
    ICommand? GetCommand(string name);
}