using System.Text.Json;
using TGBot_TW_Stock_Webhook.Extensions;
using TGBot_TW_Stock_Webhook.Interface;
using TGBot_TW_Stock_Webhook.Model.DTOs;

namespace TGBot_TW_Stock_Webhook.Services.Web
{
    public class TwStock : ITwStock
    {
        private readonly ILogger<SubscriptionService> _logger;

        public TwStock(ILogger<SubscriptionService> logger)
        {
            _logger = logger;
        }

        // TODO: 漲跌最多前50清單

        /// <summary>
        /// 當月市場成交資訊
        /// </summary>
        public async Task<List<DailyMarketInfo>> GetDailyMarketInfo()
        {
            try
            {
                var url = "https://www.twse.com.tw/rwd/zh/afterTrading/FMTQIK";
                using var httpClient = new HttpClient();
                var response = await httpClient.GetStringAsync(url);
                var stockResponse = JsonSerializer.Deserialize<TWSEApiResponse>(response);

                if (stockResponse?.Data == null || !stockResponse.Data.Any())
                    return new List<DailyMarketInfo>();

                return stockResponse.Data.Select(row => new DailyMarketInfo
                {
                    Date = Convert.ToString(row?[0]),
                    TradingVolume = Convert.ToString(row?[1])?.Replace(",", "").ParseToLong(),
                    TradingValue = Convert.ToString(row?[2]).ParseToDecimal(),
                    TransactionCount = Convert.ToString(row?[3]).ParseToInt(),
                    WeightedIndex = Convert.ToString(row?[4]).ParseToDecimal(),
                    PointChange = Convert.ToString(row?[5]).ParseToDecimal()
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetMarketOverview: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 台股收盤資訊
        /// </summary>
        /// https://www.twse.com.tw/rwd/zh/afterTrading/MI_INDEX?date=20241121&type=ALLBUT0999
        public async Task<List<StockInfo>> GetAfterTradingVolume(string? symbol)
        {
            try
            {
                var result = new List<StockInfo>();
                var date = $"{DateTime.Now.ToString("yyyyMMdd")}";

                var url = $"https://www.twse.com.tw/rwd/zh/afterTrading/MI_INDEX?date={date}&type=ALLBUT0999";
                using var httpClient = new HttpClient();
                var response = await httpClient.GetStringAsync(url);
                var stockResponse = JsonSerializer.Deserialize<TWSEApiResponse>(response);

                var stockList = stockResponse?.Tables?[8].Data;
                if (stockList == null)
                    return result;

                result = stockList.Select(row => new StockInfo
                {
                    Symbol = Convert.ToString(row?[0]),
                    Name = Convert.ToString(row?[1]),
                    TradingVolume = Convert.ToString(row?[2]),
                    TransactionCount = Convert.ToString(row?[3]),
                    TradingValue = Convert.ToString(row?[4]),
                    OpenPrice = Convert.ToString(row?[5]).ParseToDecimal(),
                    HighPrice = Convert.ToString(row?[6]).ParseToDecimal(),
                    LowPrice = Convert.ToString(row?[7]).ParseToDecimal(),
                    ClosePrice = Convert.ToString(row?[8]).ParseToDecimal(),
                    UpDownSign = Convert.ToString(row?[9]).ExtractUpDownSign(),
                    PriceChangeValue = Convert.ToString(row?[10]).ParseToDecimal(),
                }).ToList();

                if(symbol != null)
                    return result.Where(x => x.Symbol == symbol).ToList();
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetAfterTradingVolume: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 成交量前20股票
        /// </summary>
        public async Task<List<StockInfo>> GetTopVolumeItems()
        {
            try
            {
                var url = "https://www.twse.com.tw/rwd/zh/afterTrading/MI_INDEX20";
                using var httpClient = new HttpClient();
                var response = await httpClient.GetStringAsync(url);
                var stockResponse = JsonSerializer.Deserialize<TWSEApiResponse>(response);

                if (stockResponse?.Data == null || !stockResponse.Data.Any())
                    return new List<StockInfo>();

                return stockResponse.Data.Select(row => new StockInfo
                {
                    Symbol = Convert.ToString(row?[1]),
                    Name = Convert.ToString(row?[2]),
                    TradingVolume = Convert.ToString(row?[3]),
                    TransactionCount = Convert.ToString(row?[4]),
                    OpenPrice = Convert.ToString(row?[5]).ParseToDecimal(),
                    HighPrice = Convert.ToString(row?[6]).ParseToDecimal(),
                    LowPrice = Convert.ToString(row?[7]).ParseToDecimal(),
                    ClosePrice = Convert.ToString(row?[8]).ParseToDecimal(),
                    UpDownSign = Convert.ToString(row?[9]).ExtractUpDownSign(),
                    PriceChangeValue = Convert.ToString(row?[10]).ParseToDecimal(),
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetTopVolumeItems: {ex.Message}");
                throw;
            }
        }
    }
}
