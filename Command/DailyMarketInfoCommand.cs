using Telegram.Bot.Types;
using TGBot_TW_Stock_Webhook.Interface;

namespace TGBot_TW_Stock_Webhook.Services.Command
{
    public class DailyMarketInfoCommand : ICommand
    {
        public string Name => "/m";
        private readonly ITwStockBot _twStockBot;

        public DailyMarketInfoCommand(ITwStockBot twStockBot)
        {
            _twStockBot = twStockBot;
        }

        public async Task ExecuteAsync(Message message, CancellationToken cancellationToken, string? arg1, string? arg2 = null)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!string.IsNullOrEmpty(arg1))
                await _twStockBot.GetDailyMarketInfo(message, cancellationToken, Convert.ToInt16(arg1));
            else
                await _twStockBot.GetDailyMarketInfo(message, cancellationToken, 1);
        }
    }
}
