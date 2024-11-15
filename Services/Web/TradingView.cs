using PuppeteerSharp;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TGBot_TW_Stock_Webhook.Interface;

namespace TGBot_TW_Stock_Webhook.Services.Web
{
    public class TradingView
    {
        private readonly ITelegramBotClient _botClient;
        private readonly ILogger<TradingView> _logger;
        private readonly IBrowserHandlers _browserHandlers;
        private readonly ICommonService _commonService;
        private string stockUrl = "https://tw.tradingview.com/chart/?symbol=TWSE%3A";

        public TradingView(ITelegramBotClient botClient, ILogger<TradingView> logger, IBrowserHandlers browserHandlers, ICommonService commonService)
        {
            _botClient = botClient;
            _logger = logger;
            _browserHandlers = browserHandlers;
            _commonService = commonService;
        }

        /// <summary>
        /// 查詢走勢(日K)
        /// </summary>
        /// <param name="stockNumber">股票代號</param>
        /// <param name="chatID">使用者ID</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task GetChartAsync(string stockNumber, Message message, CancellationToken cancellationToken)
        {
            await _commonService.RetryAsync(async () =>
            {
                try
                {
                    //載入網頁
                    using var page = await _browserHandlers.LoadUrlAsync(stockUrl + stockNumber);

                    //等待元素載入
                    await page.WaitForNetworkIdleAsync(new WaitForNetworkIdleOptions { Timeout = 20000 });
                    var element = await page.WaitForSelectorAsync("div.chart-markup-table", new WaitForSelectorOptions { Visible = true })
                                    ?? throw new Exception("未找到指定元素:div.chart-markup-table");

                    
                    _logger.LogInformation("擷取網站中...");

                    // 截取特定元素的螢幕截圖並保存
                    Stream stream = await element.ScreenshotStreamAsync(new ElementScreenshotOptions
                    {
                        ScrollIntoView = true,    // 自動滾動到元素位置
                        OmitBackground = true,    // 可選：移除背景
                        // Type = ScreenshotType.Png // 可選：指定圖片格式
                    });

                    _logger.LogInformation("特定元素的螢幕截圖已保存");

                    await _botClient.SendPhoto(
                        chatId: message.Chat.Id,
                        photo: InputFile.FromStream(stream),
                        parseMode: ParseMode.Html,
                        cancellationToken: cancellationToken);
                    _logger.LogInformation("已傳送資訊");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"GetChartAsync:{ex.Message}");
                    throw new Exception($"GetChartAsync:{ex.Message}");
                }
            }, message, cancellationToken);
        }

        /// <summary>
        /// ️指定圖表顯示時間範圍
        /// </summary>
        /// <param name="stockNumber">股票代號</param>
        /// <param name="chatID">使用者ID</param>
        /// <param name="input">使用者輸入參數</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task GetRangeAsync(string stockNumber, Message message, string? input, CancellationToken cancellationToken)
        {
            await _commonService.RetryAsync(async () =>
            {
                try
                {
                    //載入網頁
                    using var page = await _browserHandlers.LoadUrlAsync(stockUrl + stockNumber);

                    string range;

                    #region
                    switch (input)
                    {
                        case "1d":
                            range = "1D";
                            break;
                        case "5d":
                            range = "5D";
                            break;
                        case "1m":
                            range = "1M";
                            break;
                        case "3m":
                            range = "3M";
                            break;
                        case "6m":
                            range = "6M";
                            break;
                        case "ytd":
                            range = "YTD";
                            break;
                        case "1y":
                            range = "12M";
                            break;
                        case "5y":
                            range = "60M";
                            break;
                        case "all":
                            range = "ALL";
                            break;
                        default:
                            range = "YTD";
                            break;
                    }
                    // await page.ClickAsync($"//button[value='{range}']");

                    // 等待按鈕出現
                    await page.WaitForSelectorAsync($"button[value='{range}']");

                    // 使用 CSS 選擇器語法
                    var button = await page.QuerySelectorAsync($"button[value='{range}']");
                    if (button != null)
                    {
                        await button.ClickAsync();
                    }
                    else
                    {
                        _logger.LogError($"未找到 value 為 {range} 的按鈕");
                        throw new Exception($"未找到 value 為 {range} 的按鈕");
                    }

                    //等待元素載入
                    _logger.LogInformation("等待元素載入...");
                    var element = await page.WaitForSelectorAsync("div.chart-markup-table", new WaitForSelectorOptions { Visible = true })
                                    ?? throw new Exception("未找到指定元素:div.chart-markup-table");

                    _logger.LogInformation("擷取網站中...");

                    // 截取特定元素的螢幕截圖並保存
                    Stream stream = await element.ScreenshotStreamAsync(new ElementScreenshotOptions
                    {
                        ScrollIntoView = true,    // 自動滾動到元素位置
                        OmitBackground = true,    // 可選：移除背景
                                                  // Type = ScreenshotType.Png // 可選：指定圖片格式
                    });

                    _logger.LogInformation("特定元素的螢幕截圖已保存");

                    await _botClient.SendPhoto(
                      chatId: message.Chat.Id,
                      photo: InputFile.FromStream(stream),
                      parseMode: ParseMode.Html,
                      cancellationToken: cancellationToken);
                    _logger.LogInformation("已傳送資訊");
                    #endregion
                }
                catch (Exception ex)
                {
                    _logger.LogError($"GetRangeAsync:{ex.Message}");
                    throw new Exception($"GetRangeAsync:{ex.Message}");
                }
            }, message, cancellationToken);
        }
    }
}