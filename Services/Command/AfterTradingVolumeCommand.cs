using Telegram.Bot.Types;
using TGBot_TW_Stock_Webhook.Interface;

namespace TGBot_TW_Stock_Webhook.Services.Command
{
    public class AfterTradingVolumeCommand : ICommand
    {
        public string Name => "/i";
        private readonly ITwStockBot _twStockBot;
        private readonly IBotService _botService;

        public AfterTradingVolumeCommand(ITwStockBot twStockBot, IBotService botService)
        {
            _twStockBot = twStockBot;
            _botService = botService;
        }

        public async Task ExecuteAsync(Message message, CancellationToken cancellationToken, string? symbol, string? arg2 = null)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!string.IsNullOrEmpty(symbol))
                await _twStockBot.GetAfterTradingVolume(Convert.ToInt16(symbol),message, cancellationToken);
            else
                await _botService.SendErrorMessageAsync(message, cancellationToken);
        }
    }
}
