using ExCSS;
using Newtonsoft.Json.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Text.Json;
using System.Xml;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TGBot_TW_Stock_Webhook.Enum;
using TGBot_TW_Stock_Webhook.Infrastructure.Extensions;
using TGBot_TW_Stock_Webhook.Interface.Services;
using TGBot_TW_Stock_Webhook.Model.DTOs;

namespace TGBot_TW_Stock_Webhook.Services.Bot
{
    public class TwStockBotService(ILogger<TwStockBotService> logger, IHttpClientFactory _httpClientFactory, IBotService botClien,
        ISubscriptionService subscriptionService) : ITwStockBotService
    {
        private readonly ILogger<TwStockBotService> _logger = logger;
        private readonly IBotService _botClient = botClien;
        private readonly ISubscriptionService _subscriptionService = subscriptionService;

        // TODO: 漲跌最多前50清單

        /// <summary>
        /// 當月市場成交資訊
        /// </summary>
        public async Task GetDailyMarketInfoAsync(Message message, CancellationToken cancellationToken, int? count)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                var url = "https://www.twse.com.tw/rwd/zh/afterTrading/FMTQIK";
                var stockResponse = await FetchDataAsync<TWSEApiResponse>(url, "GetDailyMarketInfo");


                if (stockResponse?.Data == null || !stockResponse.Data.Any())
                    return;

                if (count.HasValue)
                    stockResponse.Data = stockResponse.Data.TakeLast(count.Value).ToList();

                var stringBuilder = new StringBuilder();

                foreach (var row in stockResponse.Data)
                {
                    stringBuilder.AppendLine(@$"<b>{row?[0]}</b><code>");
                    stringBuilder.AppendLine(@$"成交股數：{row?[1]}");
                    stringBuilder.AppendLine(@$"成交金額：{row?[2]}");
                    stringBuilder.AppendLine(@$"成交筆數：{row?[3]}");
                    stringBuilder.AppendLine(@$"發行量加權股價指數：{row?[4]}");
                    stringBuilder.AppendLine(@$"漲跌點數：{row?[5]}");
                    stringBuilder.AppendLine(@$"</code>");
                }

                await _botClient.SendTextMessageAsync(new SendTextDto
                {
                    Message = message,
                    Text = stringBuilder.ToString(),
                    CancellationToken = cancellationToken
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "GetDailyMarketInfo");
                throw new Exception($"GetDailyMarketInfo：{ex.Message}");
            }
        }

        /// <summary>
        /// 台股收盤資訊
        /// </summary>
        public async Task GetAfterTradingVolumeAsync(string symbol, Message message, CancellationToken cancellationToken)
        {
            try
            {
                var result = new List<StockInfo>();
                var date = $"{DateTime.Now.ToString("yyyyMMdd")}";

                var url = $"https://www.twse.com.tw/rwd/zh/afterTrading/MI_INDEX?date={date}&type=ALLBUT0999";
                var stockResponse = await FetchDataAsync<TWSEApiResponse>(url, "GetAfterTradingVolume");

                var stockList = stockResponse?.Tables?[8].Data;
                if (stockList == null)
                    return;

                var stockInfo = stockList.Where(x => Convert.ToString(x?[0]) == symbol).FirstOrDefault();
                var stringBuilder = new StringBuilder();

                // 處理漲跌幅，加入表情符號
                string upDownSign = Convert.ToString(stockInfo?[9]).ExtractUpDownSign();
                decimal changeAmount = Convert.ToString(stockInfo?[10]).ParseToDecimal();
                decimal openPrice = Convert.ToString(stockInfo?[5]).ParseToDecimal();

                string emoji = upDownSign == "+" ? "📈" : upDownSign == "-" ? "📉" : "";
                // 計算漲跌幅百分比
                string percentageChange = openPrice != 0 ? $"{(changeAmount / openPrice * 100):F2}%" : "0.00%";

                stringBuilder.AppendLine(@$"<b>{stockInfo?[1]} ({stockInfo?[0]})</b>{emoji}<code>");
                stringBuilder.AppendLine(@$"成交股數：{stockInfo?[2]}");
                stringBuilder.AppendLine(@$"成交筆數：{stockInfo?[3]}");
                stringBuilder.AppendLine(@$"成交金額：{stockInfo?[4]}");
                stringBuilder.AppendLine(@$"開盤價：{openPrice}");
                stringBuilder.AppendLine(@$"收盤價：{stockInfo?[8]}");
                stringBuilder.AppendLine(@$"漲跌幅：{upDownSign}{changeAmount} ({percentageChange})");
                stringBuilder.AppendLine(@$"最高價：{stockInfo?[6]}");
                stringBuilder.AppendLine(@$"最低價：{stockInfo?[7]}");
                stringBuilder.AppendLine(@$"</code>");

                await _botClient.SendTextMessageAsync(new SendTextDto
                {
                    Message = message,
                    Text = stringBuilder.ToString(),
                    CancellationToken = cancellationToken
                });
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
        public async Task GetTopVolumeItemsAsync(Message message, CancellationToken cancellationToken)
        {
            try
            {
                var url = "https://www.twse.com.tw/rwd/zh/afterTrading/MI_INDEX20";
                var stockResponse = await FetchDataAsync<TWSEApiResponse>(url, "GetTopVolumeItems");

                if (stockResponse?.Data == null || !stockResponse.Data.Any())
                    return;

                var stringBuilder = new StringBuilder();
                stringBuilder.AppendLine($"🔝<b>今日交易量前二十</b>");
                stringBuilder.AppendLine();
                foreach (var row in stockResponse.Data)
                {
                    // 處理漲跌幅，加入表情符號
                    string upDownSign = Convert.ToString(row?[9]).ExtractUpDownSign();
                    decimal changeAmount = Convert.ToString(row?[10]).ParseToDecimal();
                    decimal openPrice = Convert.ToString(row?[5]).ParseToDecimal();

                    string emoji = upDownSign == "+" ? "📈" : upDownSign == "-" ? "📉" : "";
                    // 計算漲跌幅百分比
                    string percentageChange = openPrice != 0 ? $"{(changeAmount / openPrice * 100):F2}%" : "0.00%";
                    stringBuilder.AppendLine(@$"{emoji}<b>{row?[2]} ({row?[1]})</b><code>");
                    stringBuilder.AppendLine(@$"成交股數：{row?[3]}");
                    stringBuilder.AppendLine(@$"成交筆數：{row?[4]}");
                    stringBuilder.AppendLine(@$"開盤價：{openPrice}");
                    stringBuilder.AppendLine(@$"收盤價：{row?[8]}");
                    stringBuilder.AppendLine(@$"漲跌幅：{upDownSign}{changeAmount} ({percentageChange})");
                    stringBuilder.AppendLine(@$"最高價：{row?[6]}");
                    stringBuilder.AppendLine(@$"最低價：{row?[7]}");
                    stringBuilder.AppendLine(@$"</code>");
                }

                await _botClient.SendTextMessageAsync(new SendTextDto
                {
                    Message = message,
                    Text = stringBuilder.ToString(),
                    CancellationToken = cancellationToken
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "GetTopVolumeItems");
                throw new Exception($"GetTopVolumeItems：{ex.Message}");
            }
        }

        public async Task GetStockNewsAsync(Message message, CancellationToken cancellationToken, string? symbol)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                string url = $"https://tw.stock.yahoo.com/rss?category=tw-market";

                //載入網頁
                if (!string.IsNullOrEmpty(symbol))
                    url = $"https://tw.stock.yahoo.com/rss?s={symbol}.TW";

                XmlReader reader = XmlReader.Create(url);
                SyndicationFeed feed = SyndicationFeed.Load(reader);
                reader.Close();

                var InlineList = new List<IEnumerable<InlineKeyboardButton>>();
                foreach (var item in feed.Items.Take(5))
                {
                    InlineList.Add(new[] { InlineKeyboardButton.WithUrl(item.Title.Text, item.Links[0].Uri.ToString()) });
                }

                InlineKeyboardMarkup inlineKeyboard = new(InlineList);
                await _botClient.SendTextMessageAsync(new SendTextDto
                {
                    Message = message,
                    Text = @$"⚡️{symbol}-即時新聞",
                    ReplyMarkup = inlineKeyboard,
                    CancellationToken = cancellationToken
                });

                _logger.LogInformation("已傳送資訊");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "GetNewsAsync");
                throw new Exception($"GetNewsAsync：{ex.Message}");
            }
        }
        public async Task SubscriptionStockAsync(Message message, string stock, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                var result = await _subscriptionService.SubscriptionStockAsync(message, stock, cancellationToken);

                if (result == 0)
                {
                    await _botClient.SendTextMessageAsync(new SendTextDto
                    {
                        Message = message,
                        Text = $"訂閱失敗：{stock}，股票代碼錯誤或已訂閱",
                        CancellationToken = cancellationToken,
                    });

                    return;
                }

                // 查詢股票名稱
                using var client = _httpClientFactory.CreateClient();
                string url = $"https://www.twse.com.tw/rwd/zh/api/codeQuery?query={stock}";
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                // 解析 JSON
                var json = JObject.Parse(await response.Content.ReadAsStringAsync());
                string firstSuggestion = json?["suggestions"]?[0]?.ToString() ?? string.Empty;
                // 取出公司名
                var companyName = firstSuggestion.Split('\t')[1];

                await _botClient.SendTextMessageAsync(new SendTextDto
                {
                    Message = message,
                    Text = $"訂閱成功：{companyName} ({stock})",
                    CancellationToken = cancellationToken,
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "SubscriptionStock");
                throw new Exception($"SubscriptionStock：{ex.Message}");
            }
        }

        public async Task UnSubscriptionStockAsync(Message message, string stock, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                var result = await _subscriptionService.UnSubscriptionStockAsync(message, stock, cancellationToken);

                if (result == 0)
                {
                    await _botClient.SendTextMessageAsync(new SendTextDto
                    {
                        Message = message,
                        Text = $"取消訂閱失敗：{stock}，股票代碼錯誤或未訂閱",
                        CancellationToken = cancellationToken,
                    });

                    return;
                }

                await _botClient.SendTextMessageAsync(new SendTextDto
                {
                    Message = message,
                    Text = $"取消訂閱成功：{stock}",
                    CancellationToken = cancellationToken,
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "UnSubscriptionStock");
                throw new Exception($"UnSubscriptionStock：{ex.Message}");
            }
        }

        public async Task GetSubscriptionStockListAsync(Message message, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                var subscriptionList = await _subscriptionService.GetSubscriptionStockListAsync(message, cancellationToken);

                if (subscriptionList == null || !subscriptionList.Any())
                {
                    await _botClient.SendTextMessageAsync(new SendTextDto
                    {
                        Message = message,
                        Text = $"未訂閱任何股票",
                        CancellationToken = cancellationToken,
                    });

                    return;
                }

                var stringBuilder = new StringBuilder();
                uint num = 1;
                foreach (var subscription in subscriptionList)
                {
                    // 查詢股票名稱
                    using var client = _httpClientFactory.CreateClient();
                    string url = $"https://www.twse.com.tw/rwd/zh/api/codeQuery?query={subscription.Symbol}";
                    HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();

                    // 解析 JSON
                    var json = JObject.Parse(await response.Content.ReadAsStringAsync());
                    string firstSuggestion = json?["suggestions"]?[0]?.ToString() ?? string.Empty;
                    // 取出公司名
                    var companyName = firstSuggestion.Split('\t')[1];
                    stringBuilder.AppendLine($"<b>{num++}_{companyName} ({subscription.Symbol})</b>");
                }

                await _botClient.SendTextMessageAsync(new SendTextDto
                {
                    Message = message,
                    Text = stringBuilder.ToString(),
                    CancellationToken = cancellationToken,
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "GetSubscriptionStockList");
                throw new Exception($"GetSubscriptionStockList：{ex.Message}");
            }
        }

        public async Task SubscriptionInfoAsync(Message message, SubscriptionItemEnum subscriptionItem, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                var subscription = await _subscriptionService.SubscriptionInfoAsync(message, subscriptionItem, cancellationToken);

                if (subscription == 0)
                {
                    await _botClient.SendTextMessageAsync(new SendTextDto
                    {
                        Message = message,
                        Text = $"訂閱 {subscriptionItem.GetDescription()} 失敗",
                        CancellationToken = cancellationToken,
                    });

                    return;
                }

                await _botClient.SendTextMessageAsync(new SendTextDto
                {
                    Message = message,
                    Text = $"訂閱 {subscriptionItem.GetDescription()} 成功",
                    CancellationToken = cancellationToken,
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "SubscriptionInfo");
                throw new Exception($"SubscriptionInfo：{ex.Message}");
            }
        }

        public async Task UnSubscriptionInfoAsync(Message message, SubscriptionItemEnum subscriptionItem, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                var subscription = await _subscriptionService.UnSubscriptionInfoAsync(message, subscriptionItem, cancellationToken);

                if (subscription == 0)
                {
                    await _botClient.SendTextMessageAsync(new SendTextDto
                    {
                        Message = message,
                        Text = $"取消訂閱 {subscriptionItem.GetDescription()} 失敗",
                        CancellationToken = cancellationToken,
                    });

                    return;
                }

                await _botClient.SendTextMessageAsync(new SendTextDto
                {
                    Message = message,
                    Text = $"取消訂閱 {subscriptionItem.GetDescription()} 成功",
                    CancellationToken = cancellationToken,
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "SubscriptionInfo");
                throw new Exception($"SubscriptionInfo：{ex.Message}");
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
