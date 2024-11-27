using Telegram.Bot.Types;

namespace TGBot_TW_Stock_Webhook.Model.DTOs
{
    public class SendPhotoDto
    {
        public required Message Message { get; set; }

        public required InputFile Photo { get; set; }

        public string? Caption { get; set; } = null;

        public required CancellationToken CancellationToken { get; set; }
    }
}
