using System.Text;
using Telegram.Bot.Types;
using TGBot_TW_Stock_Webhook.Enum;
using TGBot_TW_Stock_Webhook.Interface;
using TGBot_TW_Stock_Webhook.Interface.Repository;
using TGBot_TW_Stock_Webhook.Interface.Services;
using TGBot_TW_Stock_Webhook.Model.DTOs;
using TGBot_TW_Stock_Webhook.Model.Entities;

namespace TGBot_TW_Stock_Webhook.Services
{
    public class NotificationService : INotificationService
    {
        private readonly ILogger<NotificationService> _logger;
        private readonly ISubscriptionUserStockRepository _subscriptionUserStockRepository;
        private readonly ISubscriptionUserRepository _subscriptionUserRepository;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IUserRepository _userRepository;
        private readonly ITwStockService _twStock;
        private readonly ITwStockBotService _twStockBotService;
        private readonly IBotService _botService;
        public NotificationService(ITwStockService twStock, ISubscriptionUserRepository subscriptionUserRepository, ILogger<NotificationService> logger, ISubscriptionRepository subscriptionRepository,
            IBotService botService, ISubscriptionUserStockRepository subscriptionUserStockRepository, IUserRepository userRepository,
            ITwStockBotService twStockBotService)
        {
            _twStock = twStock;
            _subscriptionUserRepository = subscriptionUserRepository;
            _logger = logger;
            _subscriptionRepository = subscriptionRepository;
            _botService = botService;
            _subscriptionUserStockRepository = subscriptionUserStockRepository;
            _userRepository = userRepository;
            _twStockBotService = twStockBotService;
        }

        public async Task SendStockInfoAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                // 取得訂閱此功能的會員清單
                var subscriptionUserList = await GetSubscriptionUserAsync(SubscriptionItemEnum.StockInfo, cancellationToken);
                if (subscriptionUserList == null || !subscriptionUserList.Any()) return;

                // 取得今日收盤資料
                var stockInfoList = await _twStock.GetAfterTradingVolumeAsync(null);
                if (stockInfoList == null || !stockInfoList.Any()) return;

                foreach (var subscriptionUser in subscriptionUserList)
                {
                    //取得使用者訂閱清單
                    var subscriptionUserStockList = await _subscriptionUserStockRepository.GetByUserIdAsync(subscriptionUser.UserId);
                    if (subscriptionUserStockList == null || !subscriptionUserStockList.Any()) return;

                    var stringBuilder = new StringBuilder();

                    foreach (var subscriptionUserStock in subscriptionUserStockList)
                    {
                        //取得股票資訊
                        var stock = stockInfoList.FirstOrDefault(x => !string.IsNullOrEmpty(x.Symbol) && subscriptionUserStock.Symbol == x.Symbol);
                        if (stock == null) return;

                        // 處理漲跌幅，加入表情符號
                        string emoji = stock.UpDownSign == "+" ? "📈" : stock.UpDownSign == "-" ? "📉" : "";
                        // 計算漲跌幅百分比
                        string percentageChange = stock.OpenPrice != 0 ? $"{stock.PriceChangeValue / stock.OpenPrice * 100:F2}%" : "0.00%";

                        stringBuilder.AppendLine(@$"<b>{stock.Name} ({stock.Symbol})</b>{emoji}<code>");
                        stringBuilder.AppendLine(@$"成交股數：{stock.TradingVolume}");
                        stringBuilder.AppendLine(@$"成交筆數：{stock.TransactionCount}");
                        stringBuilder.AppendLine(@$"成交金額：{stock.TradingValue}");
                        stringBuilder.AppendLine(@$"開盤價：{stock.OpenPrice}");
                        stringBuilder.AppendLine(@$"收盤價：{stock.ClosePrice}");
                        stringBuilder.AppendLine(@$"漲跌幅：{stock.UpDownSign}{stock.PriceChangeValue} ({percentageChange})");
                        stringBuilder.AppendLine(@$"最高價：{stock.HighPrice}");
                        stringBuilder.AppendLine(@$"最低價：{stock.LowPrice}");
                        stringBuilder.AppendLine(@$"</code>");
                    }

                    var user = await _userRepository.GetByIdAsync(subscriptionUser.UserId);
                    if (user == null) return;

                    var message = new Message
                    {
                        Chat = new Chat { Id = user.TelegramChatId }
                    };

                    await _botService.SendTextMessageAsync(new SendTextDto
                    {
                        Message = message,
                        Text = stringBuilder.ToString(),
                        CancellationToken = new CancellationToken()
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "SendStockInfo");
                throw new Exception($"SendStockInfoAsync : {ex.Message}");
            }
        }

        public async Task SendDailyMarketInfoAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                // 取得訂閱此功能的會員清單
                var subscriptionUserList = await GetSubscriptionUserAsync(SubscriptionItemEnum.DailyMarketInfo, cancellationToken);
                if (subscriptionUserList == null || !subscriptionUserList.Any()) return;

                foreach (var subscriptionUser in subscriptionUserList)
                {
                    var user = await _userRepository.GetByIdAsync(subscriptionUser.UserId);
                    if (user == null) return;

                    var message = new Message
                    {
                        Chat = new Chat { Id = user.TelegramChatId }
                    };

                    await _twStockBotService.GetDailyMarketInfoAsync(message, cancellationToken, 1);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "SendDailyMarketInfo");
                throw new Exception($"SendDailyMarketInfoAsync : {ex.Message}");
            }
        }

        public async Task SendTopVolumeItemsAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                // 取得訂閱此功能的會員清單
                var subscriptionUserList = await GetSubscriptionUserAsync(SubscriptionItemEnum.TopVolumeItems, cancellationToken);
                if (subscriptionUserList == null || !subscriptionUserList.Any()) return;

                foreach (var subscriptionUser in subscriptionUserList)
                {
                    var user = await _userRepository.GetByIdAsync(subscriptionUser.UserId);
                    if (user == null) return;

                    var message = new Message
                    {
                        Chat = new Chat { Id = user.TelegramChatId }
                    };

                    await _twStockBotService.GetTopVolumeItemsAsync(message, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "SendTopVolumeItems");
                throw new Exception($"SendTopVolumeItemsAsync : {ex.Message}");
            }
        }

        public async Task SendStockNewsAsync(CancellationToken cancellationToken)
        {
            try
            {
                // 取得訂閱此功能的會員清單
                var subscriptionUserList = await GetSubscriptionUserAsync(SubscriptionItemEnum.StockNews, cancellationToken);
                if (subscriptionUserList == null || !subscriptionUserList.Any()) return;

                foreach (var subscriptionUser in subscriptionUserList)
                {
                    //取得使用者訂閱清單
                    var subscriptionUserStockList = await _subscriptionUserStockRepository.GetByUserIdAsync(subscriptionUser.UserId);
                    if (subscriptionUserStockList == null || !subscriptionUserStockList.Any()) return;

                    var stringBuilder = new StringBuilder();

                    foreach (var subscriptionUserStock in subscriptionUserStockList)
                    {

                        var user = await _userRepository.GetByIdAsync(subscriptionUser.UserId);
                        if (user == null) return;

                        var message = new Message
                        {
                            Chat = new Chat { Id = user.TelegramChatId }
                        };

                        await _twStockBotService.GetStockNewsAsync(message, cancellationToken, subscriptionUserStock.Symbol);
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "SendStockNews");
                throw new Exception($"SendStockNewsAsync : {ex.Message}");
            }
        }

        private async Task<List<SubscriptionUser>?> GetSubscriptionUserAsync(SubscriptionItemEnum subscriptionItem, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var subscription = await _subscriptionRepository.GetByItemAsync(subscriptionItem);
            if (subscription == null) return null;

            // 取得訂閱此功能的會員清單
            return await _subscriptionUserRepository.GetBySubscriptionIdAsync(subscription.Id);
        }
    }
}