﻿using System.Text.Json;
using TGBot_TW_Stock_Webhook.Extensions;
using TGBot_TW_Stock_Webhook.Interface.Services;
using TGBot_TW_Stock_Webhook.Model.DTOs;

namespace TGBot_TW_Stock_Webhook.Services
{
    public class TwStockService(ILogger<TwStockService> _logger, IHttpClientFactory _httpClientFactory) : ITwStockService
    {
        // TODO: 漲跌最多前50清單

        /// <summary>
        /// 當月市場成交資訊
        /// </summary>
        public async Task<DailyMarketInfo?> GetDailyMarketInfoAsync()
        {
            try
            {
                var result = new List<DailyMarketInfo>();
                var url = "https://www.twse.com.tw/rwd/zh/afterTrading/FMTQIK";
                var stockResponse = await FetchDataAsync<TWSEApiResponse>(url, "GetDailyMarketInfo");

                if (stockResponse?.Data == null || !stockResponse.Data.Any())
                    return null;

                var dailyMarketInfo = stockResponse.Data.TakeLast(1).LastOrDefault();

                return new DailyMarketInfo
                {
                    Date = Convert.ToString(dailyMarketInfo?[0]),
                    TradingVolume = Convert.ToString(dailyMarketInfo?[1]),
                    TradingValue = Convert.ToString(dailyMarketInfo?[2]),
                    TransactionCount = Convert.ToString(dailyMarketInfo?[3]),
                    WeightedIndex = Convert.ToString(dailyMarketInfo?[4]),
                    PointChange = Convert.ToString(dailyMarketInfo?[5]).ParseToDecimal()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message,"GetDailyMarketInfo");
                throw new Exception($"GetDailyMarketInfo：{ex.Message}");
            }
        }

        /// <summary>
        /// 台股收盤資訊
        /// </summary>
        public async Task<List<StockInfo>?> GetAfterTradingVolumeAsync(string? symbol)
        {
            try
            {
                var result = new List<StockInfo>();
                var date = GetLastWorkingDayInTaiwanTime();

                var url = $"https://www.twse.com.tw/rwd/zh/afterTrading/MI_INDEX?date={date}&type=ALLBUT0999";
                var stockResponse = await FetchDataAsync<TWSEApiResponse>(url, "GetAfterTradingVolume");

                var stockList = stockResponse?.Tables?[8].Data;
                if (stockList == null)
                    return null;

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

                if (symbol != null)
                    return result.Where(x => x.Symbol == symbol).ToList();
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "GetAfterTradingVolume");
                throw new Exception($"GetAfterTradingVolume：{ex.Message}");
            }
        }

        /// <summary>
        /// 成交量前20股票
        /// </summary>
        public async Task<List<StockInfo>?> GetTopVolumeItemsAsync()
        {
            try
            {
                var url = "https://www.twse.com.tw/rwd/zh/afterTrading/MI_INDEX20";
                var stockResponse = await FetchDataAsync<TWSEApiResponse>(url, "GetTopVolumeItems");

                if (stockResponse?.Data == null || !stockResponse.Data.Any())
                    return null;

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
                _logger.LogError(ex.Message,"GetTopVolumeItems");
                throw new Exception($"GetTopVolumeItems：{ex.Message}");
            }
        }

        /// <summary>
        /// 取得API資料
        /// </summary>
        private async Task<T?> FetchDataAsync<T>(string url, string methodName) where T : class
        {
            try
            {
                var _httpClient = _httpClientFactory.CreateClient();
                var response = await _httpClient.GetStringAsync(url);
                return JsonSerializer.Deserialize<T>(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{methodName} 錯誤: {ex.Message}");
                throw;
            }
        }

        private string GetLastWorkingDayInTaiwanTime()
        {
            var taiwanTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Taipei Standard Time");
            var taiwanTime = TimeZoneInfo.ConvertTime(DateTime.UtcNow, taiwanTimeZone);

            switch (taiwanTime.DayOfWeek)
            {
                case DayOfWeek.Saturday:
                    taiwanTime = taiwanTime.AddDays(-1);
                    break;
                case DayOfWeek.Sunday:
                    taiwanTime = taiwanTime.AddDays(-2);
                    break;
            }

            return taiwanTime.ToString("yyyyMMdd");
        }
    }
}
