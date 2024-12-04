using Telegram.Bot.Types;
using TGBot_TW_Stock_Webhook.Enum;

namespace TGBot_TW_Stock_Webhook.Interface.Services
{
    public interface ITwStockBotService
    {
        /// <summary> 當月市場成交資訊 </summary>
        Task GetDailyMarketInfoAsync(Message message, CancellationToken cancellationToken, int? count);

        /// <summary> 成交量前20股票 </summary>
        Task GetTopVolumeItemsAsync(Message message, CancellationToken cancellationToken);

        /// <summary> 台股收盤資訊 </summary>
        Task GetAfterTradingVolumeAsync(string symbol, Message message, CancellationToken cancellationToken);

        /// <summary> 個股新聞 </summary>
        Task GetStockNewsAsync(Message message, CancellationToken cancellationToken, string? symbol);

        /// <summary> 取得訂閱清單 </summary>
        Task GetSubscriptionStockListAsync(Message message, CancellationToken cancellationToken);

        /// <summary> 訂閱股票 </summary>
        Task SubscriptionStockAsync(Message message, string stock, CancellationToken cancellationToken);

        /// <summary> 取消訂閱股票 </summary>
        Task UnSubscriptionStockAsync(Message message, string stock, CancellationToken cancellationToken);

        /// <summary> 訂閱功能 </summary>
        Task SubscriptionInfoAsync(Message message, SubscriptionItemEnum subscriptionItem, CancellationToken cancellationToken);
    }
}
