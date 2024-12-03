using Telegram.Bot.Types;
using TGBot_TW_Stock_Webhook.Model.Entities;

namespace TGBot_TW_Stock_Webhook.Interface.Services
{
    public interface ISubscriptionService
    {
        /// <summary> 訂閱 </summary>
        Task<int> SubscriptionStock(Message message, string stock, CancellationToken cancellationToken);

        /// <summary> 取消訂閱 </summary>
        Task<int> UnSubscription(Message message, string stock, CancellationToken cancellationToken);

        /// <summary> 取得訂閱清單 </summary>
        Task<List<SubscriptionUserStock>?> GetSubscriptionList(Message message, CancellationToken cancellationToken);
    }
}
