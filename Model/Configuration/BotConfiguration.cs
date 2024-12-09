namespace TGBot_TW_Stock_Webhook.Model.Configuration
{
    public class BotConfiguration
    {
        public string BotToken { get; init; } = default!;
        public Uri BotWebhookUrl { get; init; } = default!;
        public string SecretToken { get; init; } = default!;
    }
}
