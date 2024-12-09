using Telegram.Bot.Types;
using TGBot_TW_Stock_Webhook.Enum;
using TGBot_TW_Stock_Webhook.Interface.Services;

namespace TGBot_TW_Stock_Webhook.Services.Bot.Command
{
    public class UnSubscriptionInfoCommand : ICommand
    {
        public string Name => "/unsub";
        private readonly ITwStockBotService _twStockBotService;
        private readonly IBotService _botService;

        public UnSubscriptionInfoCommand(ITwStockBotService twStockBotService, IBotService botService)
        {
            _twStockBotService = twStockBotService;
            _botService = botService;
        }

        public async Task ExecuteAsync(Message message, CancellationToken cancellationToken, string? arg1, string? arg2 = null)
        {
            cancellationToken.ThrowIfCancellationRequested();

            switch (arg1)
            {
                case "t":
                    await _twStockBotService.UnSubscriptionInfoAsync(message, SubscriptionItemEnum.TopVolumeItems, cancellationToken);
                    break;
                case "d":
                    await _twStockBotService.UnSubscriptionInfoAsync(message, SubscriptionItemEnum.DailyMarketInfo, cancellationToken);
                    break;
                case "n":
                    await _twStockBotService.UnSubscriptionInfoAsync(message, SubscriptionItemEnum.StockNews, cancellationToken);
                    break;
                case "i":
                    await _twStockBotService.UnSubscriptionInfoAsync(message, SubscriptionItemEnum.StockInfo, cancellationToken);
                    break;
                default:
                    await _botService.SendErrorMessageAsync(message, cancellationToken);
                    break;
            }
        }
    }
}
