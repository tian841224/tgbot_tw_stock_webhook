using Telegram.Bot.Types;
using TGBot_TW_Stock_Webhook.Interface;
using TGBot_TW_Stock_Webhook.Interface.Services;

namespace TGBot_TW_Stock_Webhook.Command
{
    public class YahooNewsCommand : ICommand
    {
        public string Name => "/yn";
        private readonly ITwStockBotService _twStockBot;

        public YahooNewsCommand(ITwStockBotService twStockBot)
        {
            _twStockBot = twStockBot;
        }

        public async Task ExecuteAsync(Message message, CancellationToken cancellationToken, string? symbol = null, string? arg2 = null)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!string.IsNullOrEmpty(symbol))
                await _twStockBot.GetStockNews(message, cancellationToken, symbol);
            else
                await _twStockBot.GetStockNews(message, cancellationToken, null);
        }
    }
}
