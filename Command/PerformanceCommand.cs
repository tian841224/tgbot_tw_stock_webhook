using Telegram.Bot.Types;
using TGBot_TW_Stock_Webhook.Interface.Services;
using TGBot_TW_Stock_Webhook.Services.Bot;

namespace TGBot_TW_Stock_Webhook.Command
{
    public class PerformanceCommand : ICommand
    {
        public string Name => "/p";
        private readonly Cnyes _cnyes;
        private readonly IBotService _botService;

        public PerformanceCommand(Cnyes cnyes, IBotService botService)
        {
            _cnyes = cnyes;
            _botService = botService;
        }

        public async Task ExecuteAsync(Message message, CancellationToken cancellationToken, string? symbol, string? arg = null)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!string.IsNullOrEmpty(symbol))
                await _cnyes.GetPerformanceAsync(Convert.ToInt16(symbol), message, cancellationToken);
            else
                await _botService.SendErrorMessageAsync(message, cancellationToken);
        }
    }
}
