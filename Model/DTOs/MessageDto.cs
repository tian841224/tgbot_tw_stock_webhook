using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TGBot_TW_Stock_Webhook.Model.DTOs
{
    public class MessageDto
    {
        public Message Message { get; set; }
        public string Text { get; set; }
        public IReplyMarkup? ReplyMarkup { get; set; } = null ;
        public ParseMode? ParseMode { get; set; } = Telegram.Bot.Types.Enums.ParseMode.Html;
        public CancellationToken CancellationToken { get; set; }
    }
}