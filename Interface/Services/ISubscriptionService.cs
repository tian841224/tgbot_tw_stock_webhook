using Telegram.Bot.Types;
using TGBot_TW_Stock_Webhook.Enum;
using TGBot_TW_Stock_Webhook.Model.Entities;

namespace TGBot_TW_Stock_Webhook.Interface.Services
{
    public interface ISubscriptionService
    {
        /// <summary> 訂閱個股 </summary>
        Task<int> SubscriptionStock(Message message, string stock, CancellationToken cancellationToken);

        /// <summary> 取消個股訂閱 </summary>
        Task<int> UnSubscriptionStock(Message message, string stock, CancellationToken cancellationToken);

        /// <summary> 取得個股訂閱清單 </summary>
        Task<List<SubscriptionUserStock>?> GetSubscriptionStockList(Message message, CancellationToken cancellationToken);

        /// <summary> 訂閱功能 </summary>
        Task<int> SubscriptionInfo(Message message, SubscriptionItemEnum subscriptionItem, CancellationToken cancellationToken);
    }
}
