using Telegram.Bot.Types;
using TGBot_TW_Stock_Webhook.Interface.Services;

namespace TGBot_TW_Stock_Webhook.Command
{
    public class AfterTradingVolumeCommand : ICommand
    {
        public string Name => "/i";
        private readonly ITwStockBotService _twStockBot;
        private readonly IBotService _botService;

        public AfterTradingVolumeCommand(ITwStockBotService twStockBot, IBotService botService)
        {
            _twStockBot = twStockBot;
            _botService = botService;
        }

        public async Task ExecuteAsync(Message message, CancellationToken cancellationToken, string? symbol, string? arg2 = null)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!string.IsNullOrEmpty(symbol))
                await _twStockBot.GetAfterTradingVolumeAsync(symbol, message, cancellationToken);
            else
                await _botService.SendErrorMessageAsync(message, cancellationToken);
        }
    }
}
