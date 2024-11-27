using Telegram.Bot.Types;
using TGBot_TW_Stock_Webhook.Interface;

namespace TGBot_TW_Stock_Webhook.Services.Command
{
    public class TopVolumeCommand : ICommand
    {
        public string Name => "/t";
        private readonly ITwStockBot _twStockBot;
        private readonly IBotService _botService;

        public TopVolumeCommand(ITwStockBot twStockBot, IBotService botService)
        {
            _twStockBot = twStockBot;
            _botService = botService;
        }

        public async Task ExecuteAsync(Message message, CancellationToken cancellationToken, string? arg1 = null, string? arg2 = null)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!string.IsNullOrWhiteSpace(arg1) || !string.IsNullOrWhiteSpace(arg2))
            {
                await _botService.SendErrorMessageAsync(message, cancellationToken);
                return;
            }
            await _twStockBot.GetTopVolumeItems(message, cancellationToken);
        }
    }
}