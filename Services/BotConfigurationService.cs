using Telegram.Bot;
using TGBot_TW_Stock_Webhook.Interface;

namespace TGBot_TW_Stock_Webhook.Services
{
    public class BotConfigurationService : IBotConfigurationService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<BotConfigurationService> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private ITelegramBotClient _botClient;

        public BotConfigurationService(IConfiguration configuration, ILogger<BotConfigurationService> logger, IHttpClientFactory httpClientFactory, ITelegramBotClient botClient)
        {
            _configuration = configuration;
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _botClient = botClient;
        }

        public async Task UpdateBotToken(string token, CancellationToken ct = default)
        {
            try
            {
                if (string.IsNullOrEmpty(token))
                    throw new ArgumentException("Bot token cannot be empty", nameof(token));

                var httpClient = _httpClientFactory.CreateClient("tgwebhook");
                var newBotClient = new TelegramBotClient(token, httpClient);

                var me = await newBotClient.GetMe(ct);
                if (me == null)
                    throw new InvalidOperationException("Failed to validate bot token");

                _botClient = newBotClient;

                _logger.LogInformation("Bot token updated successfully for bot: {BotUsername}", me.Username);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateBotToken");
                throw;
            }
        }

        public async Task UpdateWebhookUrl(string webhookUrl, CancellationToken ct)
        {
            try
            {
                await _botClient.DeleteWebhook(cancellationToken: ct);
                _logger.LogInformation("DeleteWebhookAsync");
                await _botClient.SetWebhook(webhookUrl, allowedUpdates: [], cancellationToken: ct);
                _logger.LogInformation($"SetWebhook：{webhookUrl}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteWebhookAsync");
                throw;
            }
        }
    }
}