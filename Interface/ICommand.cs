using Telegram.Bot.Types;

public interface ICommand
{
    string Name { get; }
    Task ExecuteAsync(Message message, CancellationToken cancellationToken, string? args1 = null ,string? args2 = null);
}