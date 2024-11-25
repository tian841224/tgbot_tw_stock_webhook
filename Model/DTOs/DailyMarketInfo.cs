namespace TGBot_TW_Stock_Webhook.Model.DTOs
{
    /// <summary>
    /// 市場成交資訊
    /// </summary>
    public class DailyMarketInfo
    {
        /// <summary>
        /// 日期 (格式: 113/11/01)
        /// </summary>
        public string? Date { get; set; }

        /// <summary>
        /// 成交股數
        /// </summary>
        public string? TradingVolume { get; set; }

        /// <summary>
        /// 成交金額
        /// </summary>
        public string? TradingValue { get; set; }

        /// <summary>
        /// 成交筆數
        /// </summary>
        public string? TransactionCount { get; set; }

        /// <summary>
        /// 發行量加權股價指數
        /// </summary>
        public string? WeightedIndex { get; set; }

        /// <summary>
        /// 漲跌點數
        /// </summary>
        public decimal? PointChange { get; set; }
    }
}
