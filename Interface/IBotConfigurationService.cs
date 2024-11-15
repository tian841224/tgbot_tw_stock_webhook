namespace TGBot_TW_Stock_Webhook.Interface
{
    public interface IBotConfigurationService
    {
        /// <summary> 更新 Bot Token </summary>
        Task UpdateBotToken(string token, CancellationToken ct = default);

        /// <summary> 更新 Webhook Url </summary>
        Task UpdateWebhookUrl(string webhookUrl, CancellationToken ct);
    }
}