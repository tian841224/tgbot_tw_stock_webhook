using System.Text;
using Telegram.Bot.Types;
using TGBot_TW_Stock_Webhook.Interface.Repository;
using TGBot_TW_Stock_Webhook.Interface.Services;
using TGBot_TW_Stock_Webhook.Model.DTOs;

namespace TGBot_TW_Stock_Webhook.Services
{
    public class NotificationService : INotificationService
    {
        private readonly ILogger<NotificationService> _logger;
        private readonly IUserRepository _userService;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly ITwStockService _twStock;
        private readonly IBotService _botService;
        public NotificationService(ITwStockService twStock, IUserRepository userService, ILogger<NotificationService> logger, ISubscriptionRepository subscriptionRepository,
            IBotService botService)
        {
            _twStock = twStock;
            _userService = userService;
            _logger = logger;
            _subscriptionRepository = subscriptionRepository;
            _botService = botService;
        }

        public async Task SendStockInfo()
        {
            try
            {
                var userList = await _userService.GetAll();
                var stockInfoList = await _twStock.GetAfterTradingVolume(null);

                foreach (var user in userList)
                {
                    // 取得使用者訂閱清單
                    var subList = await _subscriptionRepository.GetByUserId(user.Id);
                    if (subList == null || subList.Count == 0) continue;

                    // 取得股票代號
                    var symbolList = subList.Select(x => x.Symbol).ToList();
                    // 取得股票資訊
                    var resultList = stockInfoList.Where(x => !string.IsNullOrEmpty(x.Symbol) && symbolList.Contains(x.Symbol)).ToList();

                    var stringBuilder = new StringBuilder();

                    foreach (var stock in resultList)
                    {
                        if (stock == null) continue;
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
                    };

                    var message = new Message
                    {
                        Chat = new Chat { Id = user.Id }
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
            }
        }

        public async Task SendDailyMarketInfo()
        {
            try
            {
                var userList = await _userService.GetAll();
                var stockInfoList = await _twStock.GetDailyMarketInfo();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "SendDailyMarketInfo");
            }
        }

        public async Task SendTopVolumeItems()
        {
            try
            {
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "SendTopVolumeItems");
            }
        }

        public async Task SendStockNews()
        {
            try
            {

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "SendStockNews");
            }
        }
    }
}