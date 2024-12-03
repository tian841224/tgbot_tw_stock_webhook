using Telegram.Bot.Types;
using TGBot_TW_Stock_Webhook.Interface.Services;

namespace TGBot_TW_Stock_Webhook.Command
{
    public class GetSubscriptionListCommand : ICommand
    {
        public string Name => "/g";
        private readonly ITwStockBotService _twStockBotService;
        private readonly IBotService _botService;

        public GetSubscriptionListCommand(ITwStockBotService twStockBotService, IBotService botService)
        {
            _twStockBotService = twStockBotService;
            _botService = botService;
        }

        public async Task ExecuteAsync(Message message, CancellationToken cancellationToken, string? symbol, string? arg = null)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await _twStockBotService.GetSubscriptionList(message, cancellationToken);
        }
    }
}
