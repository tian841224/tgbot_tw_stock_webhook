using Telegram.Bot.Types;
using TGBot_TW_Stock_Webhook.Interface.Services;

namespace TGBot_TW_Stock_Webhook.Command
{
    public class DailyMarketInfoCommand : ICommand
    {
        public string Name => "/m";
        private readonly ITwStockBotService _twStockBot;

        public DailyMarketInfoCommand(ITwStockBotService twStockBot)
        {
            _twStockBot = twStockBot;
        }

        public async Task ExecuteAsync(Message message, CancellationToken cancellationToken, string? arg1, string? arg2 = null)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                var count = !string.IsNullOrEmpty(arg1) && Int16.TryParse(arg1, out short result)
                             ? result
                              : (short)1;
                await _twStockBot.GetDailyMarketInfoAsync(message, cancellationToken, count);
            }
            catch
            {
                throw;
            }
        }
    }
}
