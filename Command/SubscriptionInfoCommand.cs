using Telegram.Bot.Types;
using TGBot_TW_Stock_Webhook.Enum;
using TGBot_TW_Stock_Webhook.Extensions;
using TGBot_TW_Stock_Webhook.Interface.Services;

namespace TGBot_TW_Stock_Webhook.Command
{
    public class SubscriptionInfoCommand : ICommand
    {
        public string Name => "/add";
        private readonly ITwStockBotService _twStockBotService;
        private readonly IBotService _botService;

        public SubscriptionInfoCommand(ITwStockBotService twStockBotService, IBotService botService)
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
                    await _twStockBotService.SubscriptionInfo(message, SubscriptionItemEnum.TopVolumeItems, cancellationToken);
                    break;
                case "d":
                    await _twStockBotService.SubscriptionInfo(message, SubscriptionItemEnum.DailyMarketInfo, cancellationToken);
                    break;
                case "n":
                    await _twStockBotService.SubscriptionInfo(message, SubscriptionItemEnum.StockNews, cancellationToken);
                    break;
                default:
                    await _botService.SendErrorMessageAsync(message, cancellationToken);
                    break;
            }
        }
    }
}
