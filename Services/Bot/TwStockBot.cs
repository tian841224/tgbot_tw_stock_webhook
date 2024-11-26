using System.Text;
using System.Text.Json;
using Telegram.Bot.Types;
using TGBot_TW_Stock_Webhook.Extensions;
using TGBot_TW_Stock_Webhook.Interface;
using TGBot_TW_Stock_Webhook.Model.DTOs;

namespace TGBot_TW_Stock_Webhook.Services.Bot
{
    public class TwStockBot(ILogger<TwStockBot> logger, IHttpClientFactory httpClientFactory, IBotService botClien) : ITwStockBot
    {
        private readonly ILogger<TwStockBot> _logger = logger;
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
        private readonly IBotService _botClient = botClien;

        // TODO: 漲跌最多前50清單

        /// <summary>
        /// 當月市場成交資訊
        /// </summary>
        public async Task GetDailyMarketInfo(Message message, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                var url = "https://www.twse.com.tw/rwd/zh/afterTrading/FMTQIK";
                var stockResponse = await FetchDataAsync<TWSEApiResponse>(url, "GetDailyMarketInfo");


                if (stockResponse?.Data == null || !stockResponse.Data.Any())
                    return;

                var stringBuilder = new StringBuilder();

                foreach (var row in stockResponse.Data)
                {
                    stringBuilder.AppendLine(@$"<b>-{Convert.ToString(row?[0])}-</b>");
                    stringBuilder.AppendLine(@$"<code>成交股數：{Convert.ToString(row?[1])}</code>");
                    stringBuilder.AppendLine(@$"<code>成交金額：{Convert.ToString(row?[2])}</code>");
                    stringBuilder.AppendLine(@$"<code>成交筆數：{Convert.ToString(row?[3])}</code>");
                    stringBuilder.AppendLine(@$"<code>發行量加權股價指數：{Convert.ToString(row?[4])}</code>");
                    stringBuilder.AppendLine(@$"<code>漲跌點數：{Convert.ToString(row?[5])}</code>");
                    stringBuilder.AppendLine(@$"<b>----------</b>");
                }

                await _botClient.SendTextMessageAsync(new MessageDto
                {
                    Message = message,
                    Text = stringBuilder.ToString(),
                    CancellationToken = cancellationToken
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetDailyMarketInfo: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 台股收盤資訊
        /// </summary>
        public async Task<List<StockInfo>> GetAfterTradingVolume(string? symbol)
        {
            try
            {
                var result = new List<StockInfo>();
                var date = $"{DateTime.Now.ToString("yyyyMMdd")}";

                var url = $"https://www.twse.com.tw/rwd/zh/afterTrading/MI_INDEX?date={date}&type=ALLBUT0999";
                var stockResponse = await FetchDataAsync<TWSEApiResponse>(url, "GetAfterTradingVolume");

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

                if (symbol != null)
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
                var stockResponse = await FetchDataAsync<TWSEApiResponse>(url, "GetTopVolumeItems");

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
    }
}
