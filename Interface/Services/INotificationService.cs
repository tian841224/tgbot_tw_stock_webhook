namespace TGBot_TW_Stock_Webhook.Interface.Services
{
    public interface INotificationService
    {
        /// <summary> 推播使用者訂閱股票資訊 </summary>
        Task SendStockInfoAsync(CancellationToken cancellationToken);

        /// <summary> 推播當日市場行情 </summary>
        Task SendDailyMarketInfoAsync(CancellationToken cancellationToken);

        /// <summary> 推播當日成交量前20股票 </summary>
        Task SendTopVolumeItemsAsync(CancellationToken cancellationToken);

        /// <summary> 推播使用者訂閱股票新聞 </summary>
        Task SendStockNewsAsync(CancellationToken cancellationToken);

    }
}
