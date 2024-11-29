using Telegram.Bot.Types;

namespace TGBot_TW_Stock_Webhook.Interface
{
    public interface ICommonService
    {
        /// <summary> 方法重試 </summary>
        Task RetryAsync(Func<Task> action, Message message, CancellationToken cancellationToken);
    }
}