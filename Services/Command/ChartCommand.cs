using Telegram.Bot.Types;
using TGBot_TW_Stock_Webhook.Interface;
using TGBot_TW_Stock_Webhook.Services.Bot;

namespace TGBot_TW_Stock_Webhook.Services.Command
{
    public class ChartCommand : ICommand
    {
        public string Name => "/c";
        private readonly TradingView _tradingView;
        private readonly IBotService _botService;

        public ChartCommand(TradingView tradingView, IBotService botService)
        {
            _tradingView = tradingView;
            _botService = botService;
        }

        public async Task ExecuteAsync(Message message, CancellationToken cancellationToken, string? symbol, string? arg = null)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!string.IsNullOrEmpty(symbol) && string.IsNullOrEmpty(arg))
                await _tradingView.GetChartAsync(symbol, message, cancellationToken);
            else if (!string.IsNullOrEmpty(symbol) && !string.IsNullOrEmpty(arg))
                await _tradingView.GetChartAsync(symbol, message, cancellationToken, arg);
            else
                await _botService.SendErrorMessageAsync(message, cancellationToken);
        }
    }
}
