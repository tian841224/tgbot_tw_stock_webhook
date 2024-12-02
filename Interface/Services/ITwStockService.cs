using TGBot_TW_Stock_Webhook.Model.DTOs;

namespace TGBot_TW_Stock_Webhook.Interface.Services
{
    public interface ITwStockService
    {
        /// <summary>
        /// 取得成交量前20
        /// </summary>
        Task<List<StockInfo>?> GetTopVolumeItems();

        /// <summary>
        /// 當月市場成交資訊
        /// </summary>
        Task<DailyMarketInfo?> GetDailyMarketInfo();

        /// <summary>
        /// 台股收盤資訊
        /// </summary>
        /// <param name="stock">股票代號</param>
        Task<List<StockInfo>?> GetAfterTradingVolume(string? symbol);
    }
}
