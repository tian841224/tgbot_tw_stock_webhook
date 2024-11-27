using Telegram.Bot.Types;
using TGBot_TW_Stock_Webhook.Interface;
using TGBot_TW_Stock_Webhook.Services.Bot;

public class KlineCommand : ICommand
{
    public string Name => "/k";
    private readonly Cnyes _cnyes;
    private readonly IBotService _botService;

    public KlineCommand(Cnyes cnyes, IBotService botService)
    {
        _cnyes = cnyes;
        _botService = botService;
    }

    public async Task ExecuteAsync(Message message, CancellationToken cancellationToken, string? symbol, string? arg = null)
    {
        cancellationToken.ThrowIfCancellationRequested();

        string? range = null;
        if (!string.IsNullOrEmpty(arg))
             range = GetKRange(arg);

        if (!string.IsNullOrEmpty(symbol))
            await _cnyes.GetKlineAsync(Convert.ToInt16(symbol), message, cancellationToken , range);
        else
            await _botService.SendErrorMessageAsync(message, cancellationToken);
    }

    private string GetKRange(string input)
    {
        return input.ToLowerInvariant() switch
        {
            "h" => "分時",
            "d" => "日K",
            "w" => "週K",
            "m" => "月K",
            "5m" => "5分",
            "10m" => "10分",
            "15m" => "15分",
            "30m" => "30分",
            "60m" => "60分",
            _ => "日K"
        };
    }
}