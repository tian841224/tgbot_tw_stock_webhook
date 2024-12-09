using Telegram.Bot.Types;
using TGBot_TW_Stock_Webhook.Interface.Services;

namespace TGBot_TW_Stock_Webhook.Services.Bot.Command
{
    public class UnSubscriptionStockCommand : ICommand
    {
        public string Name => "/del";
        private readonly ITwStockBotService _twStockBotService;
        private readonly IBotService _botService;

        public UnSubscriptionStockCommand(ITwStockBotService twStockBotService, IBotService botService)
        {
            _twStockBotService = twStockBotService;
            _botService = botService;
        }

        public async Task ExecuteAsync(Message message, CancellationToken cancellationToken, string? symbol, string? arg = null)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!string.IsNullOrEmpty(symbol))
                await _twStockBotService.UnSubscriptionStockAsync(message, symbol, cancellationToken);
            else
                await _botService.SendErrorMessageAsync(message, cancellationToken);
        }
    }
}
