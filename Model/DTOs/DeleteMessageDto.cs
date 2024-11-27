using Telegram.Bot.Types;

namespace TGBot_TW_Stock_Webhook.Model.DTOs
{
    public class DeleteMessageDto
    {
        public required Message Message { get; set; }

        public required Message Reply { get; set; }

        public required CancellationToken CancellationToken { get; set; }
    }
}