using Telegram.Bot.Types;
using TGBot_TW_Stock_Webhook.Interface.Services;

namespace TGBot_TW_Stock_Webhook.Services.Bot.Command
{
    public class GetSubscriptionStockListCommand : ICommand
    {
        public string Name => "/g";
        private readonly ITwStockBotService _twStockBotService;

        public GetSubscriptionStockListCommand(ITwStockBotService twStockBotService)
        {
            _twStockBotService = twStockBotService;
        }

        public async Task ExecuteAsync(Message message, CancellationToken cancellationToken, string? symbol, string? arg = null)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await _twStockBotService.GetSubscriptionStockListAsync(message, cancellationToken);
        }
    }
}
