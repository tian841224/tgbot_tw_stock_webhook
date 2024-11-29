using Telegram.Bot.Types;
using TGBot_TW_Stock_Webhook.Model.DTOs;

namespace TGBot_TW_Stock_Webhook.Interface
{
    public interface IBotService
    {
        /// <summary>傳送訊息</summary>
        Task<Message> SendTextMessageAsync(SendTextDto dto);

        /// <summary>傳送圖片</summary>
        Task<Message> SendPhotoAsync(SendPhotoDto dto);

        /// <summary>傳送Hello訊息</summary>
        Task<Message> SendHelloMessageAsync(Message message, CancellationToken cancellationToken);

        /// <summary>傳送等待訊息</summary>
        Task<Message> SendWaitMessageAsync(Message message, CancellationToken cancellationToken);

        /// <summary>傳送指令錯誤訊息</summary>
        Task SendErrorMessageAsync(Message message, CancellationToken cancellationToken);

        /// <summary>刪除訊息</summary>
        Task DeleteMessageAsync(DeleteMessageDto dto);

        /// <summary>錯誤通知</summary>
        Task ErrorNotify(Message message, string errorMessage, CancellationToken cancellationToken);
    }
}