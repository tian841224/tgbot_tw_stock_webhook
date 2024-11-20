using Telegram.Bot.Types;

namespace TGBot_TW_Stock_Webhook.Model.DTOs
{
    public class DeleteMessageDto
    {
        public Message Message { get; set; }

        public Message Reply { get; set; }

        public CancellationToken CancellationToken { get; set; }
    }
}