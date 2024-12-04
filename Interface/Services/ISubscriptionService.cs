using Telegram.Bot.Types;
using TGBot_TW_Stock_Webhook.Enum;
using TGBot_TW_Stock_Webhook.Model.Entities;

namespace TGBot_TW_Stock_Webhook.Interface.Services
{
    public interface ISubscriptionService
    {
        /// <summary> 訂閱個股 </sumㄔmary>
        Task<int> SubscriptionStockAsync(Message message, string stock, CancellationToken cancellationToken);

        /// <summary> 取消個股訂閱 </summary>
        Task<int> UnSubscriptionStockAsync(Message message, string stock, CancellationToken cancellationToken);

        /// <summary> 取得個股訂閱清單 </summary>
        Task<List<SubscriptionUserStock>?> GetSubscriptionStockListAsync(Message message, CancellationToken cancellationToken);

        /// <summary> 訂閱功能 </summary>
        Task<int> SubscriptionInfoAsync(Message message, SubscriptionItemEnum subscriptionItem, CancellationToken cancellationToken);
    }
}
