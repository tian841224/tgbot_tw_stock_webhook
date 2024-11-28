using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TGBot_TW_Stock_Webhook.Model.DTOs
{
    public class BotMessageDto
    {
        public required Message Message { get; set; }
        public IReplyMarkup? ReplyMarkup { get; set; } = null;
        public ParseMode? ParseMode { get; set; } = Telegram.Bot.Types.Enums.ParseMode.Html;
        public required CancellationToken CancellationToken { get; set; }
    }
}
