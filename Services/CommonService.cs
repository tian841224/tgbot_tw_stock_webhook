using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TGBot_TW_Stock_Webhook.Interface;

namespace TGBot_TW_Stock_Webhook.Services
{
    public class CommonService : ICommonService
    {
        private readonly ILogger<CommonService> _logger;
        private readonly IBrowserHandlers _browserHandlers;
        private readonly ITelegramBotClient _botClient;
        private readonly TimeSpan delay = TimeSpan.FromSeconds(3); // 每次重試的延遲時間
        private readonly int maxRetries = 3; // 最大重試次數

        public CommonService(ILogger<CommonService> logger, IBrowserHandlers browserHandlers, ITelegramBotClient botClient)
        {
            _logger = logger;
            _browserHandlers = browserHandlers;
            _botClient = botClient;
        }


        ///  <summary> 方法重試 </summary>
        public async Task RetryAsync(Func<Task> action, Message message, CancellationToken cancellationToken)
        {
            int retryCount = 0;
            while (retryCount < maxRetries)
            {
                try
                {
                    await action();
                    break;
                }
                catch (Exception ex)
                {
                    retryCount++;
                    _logger.LogWarning($"嘗試 {retryCount} 次失敗：{ex.Message}");
                    if (retryCount > maxRetries)
                    {
                        _logger.LogInformation($"已達最大重試次數 ({maxRetries})，拋出例外。");
                        await _botClient.SendMessage(
                           text: "因機器人部屬於雲端，有時會無法讀取網頁，請將程式部屬至本機執行。",
                           chatId: message.Chat.Id,
                           parseMode: ParseMode.Html,
                           cancellationToken: cancellationToken);
                        _logger.LogInformation("已傳送資訊");

                        // 格式化方法名稱
                        string methodName = action.Method.Name;
                        if (methodName.Contains('<') && methodName.Contains('>'))
                        {
                            // 提取 "<" 和 ">" 之間的部分
                            int startIndex = methodName.IndexOf('<') + 1;
                            int endIndex = methodName.IndexOf('>');
                            methodName = methodName.Substring(startIndex, endIndex - startIndex);
                        }

                        throw new Exception($"{methodName}：{ex.Message}");
                    }
                    _logger.LogInformation($"等待 {delay.TotalSeconds} 秒後重試...");
                    await Task.Delay(delay, cancellationToken);
                    await _browserHandlers.ReleaseBrowser();
                }
            }
        }
    }
}