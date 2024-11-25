namespace TGBot_TW_Stock_Webhook.Model.DTOs
{
    /// <summary>
    /// 股票資訊
    /// </summary>
    public class StockInfo
    {
        /// <summary> 股票代號 </summary>
        public string? Symbol { get; set; }

        /// <summary> 股票名稱 </summary>
        public string? Name { get; set; }

        /// <summary> 成交股數 </summary>
        public string? TradingVolume { get; set; }

        /// <summary> 成交筆數 </summary>
        public string? TransactionCount { get; set; }

        /// <summary> 成交金額 </summary>
        public string? TradingValue { get; set; }

        /// <summary> 開盤價 </summary>
        public decimal? OpenPrice { get; set; }

        /// <summary> 最高價 </summary>
        public decimal? HighPrice { get; set; }

        /// <summary> 最低價 </summary>
        public decimal? LowPrice { get; set; }

        /// <summary> 收盤價 </summary>
        public decimal? ClosePrice { get; set; }

        /// <summary> 漲跌(+/-) </summary>
        public string? UpDownSign { get; set; }

        /// <summary> 漲跌價差 </summary>
        public decimal? PriceChangeValue { get; set; }

        /// <summary> 本益比 </summary>
        public decimal? PERatio { get; set; }
    }
}
