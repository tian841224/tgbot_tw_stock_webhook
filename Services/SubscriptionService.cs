﻿using System.Text.Json;
using Telegram.Bot.Types;
using TGBot_TW_Stock_Webhook.Enum;
using TGBot_TW_Stock_Webhook.Interface.Repository;
using TGBot_TW_Stock_Webhook.Interface.Services;
using TGBot_TW_Stock_Webhook.Model.Entities;

namespace TGBot_TW_Stock_Webhook.Services
{
    /// <summary>
    /// 訂閱功能
    /// </summary>
    public class SubscriptionService(ILogger<SubscriptionService> logger, IUserRepository userRepository,
        ISubscriptionUserRepository subscriptionUserRepository, ISubscriptionRepository subscriptionRepository,
        ISubscriptionUserStockRepository subscriptionUserStockRepository) : ISubscriptionService
    {
        private readonly ILogger<SubscriptionService> _logger = logger;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly ISubscriptionRepository _subscriptionRepository = subscriptionRepository;
        private readonly ISubscriptionUserRepository _subscriptionUserRepository = subscriptionUserRepository;
        private readonly ISubscriptionUserStockRepository _subscriptionUserStockRepository = subscriptionUserStockRepository;

        public async Task<int> SubscriptionStock(Message message, string stock, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                // 取得使用者
                var user = await _userRepository.GetByChatIdAsync(message.Chat.Id);

                if (user == null)
                {
                    user = new Model.Entities.User
                    {
                        UserId = message.Chat.Id,
                        UserName = message.Chat.Username,
                        Status = true,
                    };
                    await _userRepository.AddAsync(user);
                }
                    
                if (user.Status == false) throw new Exception("本帳號無法使用");

                // 判斷使用者是否有訂閱個股推送功能
                var subscription = await _subscriptionRepository.GetByItemAsync(SubscriptionItemEnum.StockInfo);

                if (subscription == null)
                {
                    subscription = new Subscription
                    {
                        Item = SubscriptionItemEnum.StockInfo,
                        Description = "個股資訊",
                        Status = true,
                    };
                    await _subscriptionRepository.AddAsync(subscription);
                }

                var subscriptionUser = await _subscriptionUserRepository.GetBySubscriptionIdAsync(subscription.Id);
                if (subscriptionUser == null)
                {
                    await subscriptionUserRepository.AddAsync(new SubscriptionUser
                    {
                        UserId = user.Id,
                        SubscriptionId = subscription.Id
                    });
                }

                var subscriptionUserStock = await _subscriptionUserStockRepository.GetBySymbolAsync(stock);
                if (subscriptionUserStock == null)
                {
                    // 判斷股票代號是否存在
                    using var client = new HttpClient();
                    string url = $"https://www.twse.com.tw/rwd/zh/afterTrading/STOCK_DAY?stockNo={stock}";
                    HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    string jsonString = await response.Content.ReadAsStringAsync();
                    var stockData = JsonSerializer.Deserialize<StockData>(jsonString);

                    // 找不到股票資訊
                    if (stockData?.total == 0)
                        throw new Exception($"訂閱失敗：{stock}，股票代碼錯誤");

                    return await _subscriptionUserStockRepository.AddAsync(new SubscriptionUserStock
                    {
                        UserId = user.Id,
                        Symbol = stock
                    });
                }

                return 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Subscription Error");
                throw;
            }
        }

        public async Task<int> UnSubscriptionStock(Message message, string stock, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                // 取得使用者
                var user = await _userRepository.GetByChatIdAsync(message.Chat.Id);
                if (user == null || user.Status == false) throw new Exception("本帳號無法使用");

                // 判斷使用者是否有訂閱個股
                var subscriptionUserStock = await _subscriptionUserStockRepository.GetByUserIdAndSymbolAsync(user.Id, stock);

                if (subscriptionUserStock == null) return 0;

                // 刪除訂閱
                return await _subscriptionUserStockRepository.DeleteAsync(subscriptionUserStock);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UnSubscription Error");
                throw new Exception($"UnSubscription：{ex.Message}");
            }
        }

        public async Task<List<SubscriptionUserStock>?> GetSubscriptionList(Message message, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                // 取得使用者
                var user = await _userRepository.GetByChatIdAsync(message.Chat.Id);
                if (user == null) throw new Exception("本帳號無法使用");

                return await _subscriptionUserStockRepository.GetByUserIdAsync(user.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetSubscriptionList Error");
                throw new Exception($"GetSubscriptionList：{ex.Message}");
            }
        }

        private class StockData
        {
            public string stat { get; set; } = string.Empty;
            public int total { get; set; }
        }
    }
}