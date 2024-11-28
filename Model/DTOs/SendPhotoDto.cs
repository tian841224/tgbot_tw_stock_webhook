using Telegram.Bot.Types;

namespace TGBot_TW_Stock_Webhook.Model.DTOs
{
    public class SendPhotoDto : BotMessageDto
    {
        public required InputFile Photo { get; set; }

        public string? Caption { get; set; } = null;
    }
}
