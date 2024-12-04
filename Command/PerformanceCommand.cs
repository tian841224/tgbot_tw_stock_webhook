using Telegram.Bot.Types;
using TGBot_TW_Stock_Webhook.Interface.Services;
using TGBot_TW_Stock_Webhook.Services.Bot;

namespace TGBot_TW_Stock_Webhook.Command
{
    public class PerformanceCommand : ICommand
    {
        public string Name => "/p";
        private readonly Cnyes _cnyes;
        private readonly IBotService _botService;

        public PerformanceCommand(Cnyes cnyes, IBotService botService)
        {
            _cnyes = cnyes;
            _botService = botService;
        }

        public async Task ExecuteAsync(Message message, CancellationToken cancellationToken, string? symbol, string? arg = null)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                if (!string.IsNullOrEmpty(symbol))
                {
                    var count = !string.IsNullOrEmpty(symbol) && Int16.TryParse(symbol, out var result)
                            ? result
                            : 1;

                    await _cnyes.GetPerformanceAsync(count, message, cancellationToken);

                }
                else
                    await _botService.SendErrorMessageAsync(message, cancellationToken);
            }
            catch
            {
                throw;
            }
        }
    }
}
