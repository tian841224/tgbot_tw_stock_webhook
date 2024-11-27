using Telegram.Bot.Types;
using TGBot_TW_Stock_Webhook.Interface;

namespace TGBot_TW_Stock_Webhook.Services.Command
{
    public class YahooNewsCommand : ICommand
    {
        public string Name => "/yn";
        private readonly ITwStockBot _twStockBot;

        public YahooNewsCommand(ITwStockBot twStockBot)
        {
            _twStockBot = twStockBot;
        }

        public async Task ExecuteAsync(Message message, CancellationToken cancellationToken, string? symbol = null, string? arg2 = null)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!string.IsNullOrEmpty(symbol))
                await _twStockBot.GetStockNews(message, cancellationToken, Convert.ToInt16(symbol));
            else
                await _twStockBot.GetStockNews(message, cancellationToken , null);
        }
    }
}
