using Telegram.Bot.Types;
using TGBot_TW_Stock_Webhook.Model.Entities;

namespace TGBot_TW_Stock_Webhook.Interface
{
    public interface ISubscriptionService
    {
        /// <summary> 訂閱 </summary>
        Task Subscription(Message message, string stock, CancellationToken cancellationToken);
        /// <summary> 取消訂閱 </summary>
        Task UnSubscription(Message message, string stock, CancellationToken cancellationToken);
        /// <summary> 取得訂閱清單 </summary>
        Task<List<Subscription>?> GetSubscriptionList(Message message, CancellationToken cancellationToken);
    }
}
