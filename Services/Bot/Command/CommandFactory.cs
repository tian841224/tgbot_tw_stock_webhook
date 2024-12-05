public class CommandFactory : ICommandFactory
{
    private readonly IDictionary<string, ICommand> _commands;

    public CommandFactory(IEnumerable<ICommand> commands)
    {
        _commands = commands.ToDictionary(c => c.Name, StringComparer.OrdinalIgnoreCase);
    }

    public ICommand? GetCommand(string name)
    {
        _commands.TryGetValue(name, out var command);
        return command;
    }
}