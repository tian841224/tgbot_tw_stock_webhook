namespace TGBot_TW_Stock_Webhook.Model.DTOs
{
    public class SendTextDto : BotMessageDto
    {
        public required string Text { get; set; }
    }
}