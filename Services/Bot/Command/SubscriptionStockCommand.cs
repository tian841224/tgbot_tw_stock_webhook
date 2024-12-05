using Telegram.Bot.Types;
using TGBot_TW_Stock_Webhook.Interface.Services;

namespace TGBot_TW_Stock_Webhook.Command
{
    public class SubscriptionStockCommand : ICommand
    {
        public string Name => "/sub";
        private readonly ITwStockBotService _twStockBotService;
        private readonly IBotService _botService;

        public SubscriptionStockCommand(ITwStockBotService twStockBotService, IBotService botService)
        {
            _twStockBotService = twStockBotService;
            _botService = botService;
        }

        public async Task ExecuteAsync(Message message, CancellationToken cancellationToken, string? symbol, string? arg = null)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!string.IsNullOrEmpty(symbol))
                await _twStockBotService.SubscriptionStockAsync(message, symbol, cancellationToken);
            else
                await _botService.SendErrorMessageAsync(message, cancellationToken);
        }
    }
}
