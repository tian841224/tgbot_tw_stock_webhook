using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Eventing.Reader;
using System.Text;
using System.Text.Json;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Types;
using TGBot_TW_Stock_Webhook.Data;
using TGBot_TW_Stock_Webhook.Interface;
using TGBot_TW_Stock_Webhook.Model.DTOs;
using TGBot_TW_Stock_Webhook.Model.Entities;
using User = TGBot_TW_Stock_Webhook.Model.Entities.User;

namespace TGBot_TW_Stock_Webhook.Services
{
    /// <summary>
    /// 訂閱功能
    /// </summary>
    public class SubscriptionService(ILogger<SubscriptionService> logger, AppDbContext context, IBotService botService) : ISubscriptionService
    {
        private readonly ILogger<SubscriptionService> _logger = logger;
        private readonly AppDbContext _context = context;
        private readonly IBotService _botService = botService;


        public async Task Subscription(Message message, string stock, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                // 取得使用者
                var user = await GetUser(message, cancellationToken);
                if (user == null) throw new Exception("本帳號無法使用");

                // 判斷是否已經訂閱過
                var subscription = await _context.Subscriptions.Where(x => x.UserId == user.Id && x.Stock == stock).FirstOrDefaultAsync(cancellationToken);
                if (subscription != null)
                {
                    subscription.IsDelete = false;
                    _context.Subscriptions.Update(subscription);
                }
                else
                {
                    // 判斷股票代號是否存在
                    // https://www.twse.com.tw/rwd/zh/afterTrading/STOCK_DAY?stockNo=0050
                    using var client = new HttpClient();
                    string url = $"https://www.twse.com.tw/rwd/zh/afterTrading/STOCK_DAY?stockNo={stock}";
                    HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();

                    string jsonString = await response.Content.ReadAsStringAsync();
                    var stockData = JsonSerializer.Deserialize<StockData>(jsonString);

                    // 找不到股票資訊
                    if (stockData?.total == 0)
                    {
                        await _botService.SendTextMessageAsync(new MessageDto
                        {
                            Message = message,
                            Text = $"訂閱失敗：{stock}，股票代碼錯誤",
                            CancellationToken = cancellationToken,
                        });
                        throw new Exception($"訂閱失敗：{stock}，股票代碼錯誤");
                    }

                    // 新增訂閱
                    subscription = new Subscription
                    {
                        UserId = user.Id,
                        Stock = stock,
                    };
                    _context.Subscriptions.Add(subscription);
                }

                var count = await _context.SaveChangesAsync(cancellationToken);
                if (count == 0)
                {
                    await _botService.SendTextMessageAsync(new MessageDto
                    {
                        Message = message,
                        Text = $"訂閱失敗：{stock}，使用 /list 指令確認是否已訂閱",
                        CancellationToken = cancellationToken,
                    });
                }
                else
                {
                    await _botService.SendTextMessageAsync(new MessageDto
                    {
                        Message = message,
                        Text = $"訂閱成功：{stock}",
                        CancellationToken = cancellationToken,
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Subscription Error");
                throw new Exception($"Subscription：{ex.Message}");
            }
        }

        public async Task UnSubscription(Message message, string stock, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                // 取得使用者
                var user = await GetUser(message, cancellationToken);
                if (user == null) throw new Exception("本帳號無法使用");

                // 判斷是否已經訂閱過
                var subscription = await _context.Subscriptions.Where(x => x.UserId == user.Id && x.Stock == stock).FirstOrDefaultAsync(cancellationToken);
                if (subscription == null) return;

                // 刪除訂閱
                subscription.IsDelete = true;
                _context.Subscriptions.Update(subscription);

                var count = await _context.SaveChangesAsync(cancellationToken);
                if (count == 0)
                {
                    await _botService.SendTextMessageAsync(new MessageDto
                    {
                        Message = message,
                        Text = $"取消訂閱失敗：{stock}，使用 /list 指令確認是否已訂閱",
                        CancellationToken = cancellationToken,
                    });
                }
                else
                {
                    await _botService.SendTextMessageAsync(new MessageDto
                    {
                        Message = message,
                        Text = $"取消訂閱成功：{stock}",
                        CancellationToken = cancellationToken,
                    });
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UnSubscription Error");
                throw new Exception($"UnSubscription：{ex.Message}");
            }
        }

        public async Task<List<Subscription>?> GetSubscriptionList(Message message, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // 取得使用者
            var user = await GetUser(message, cancellationToken);
            if (user == null) throw new Exception("本帳號無法使用");

            try
            {
                // 判斷是否已經訂閱過
                var subscription = await _context.Subscriptions.Where(x => x.UserId == user.Id && x.IsDelete == false).ToListAsync(cancellationToken);
                if (subscription == null || subscription.Count() == 0)
                {
                    await _botService.SendTextMessageAsync(
                   new MessageDto
                   {
                       Message = message,
                       Text = "訂閱清單為空",
                       CancellationToken = cancellationToken,
                   });
                }
                else
                {
                    StringBuilder sb = new StringBuilder();
                    subscription.ForEach(item => sb.AppendLine($"{item.Id}.{item.Stock}"));

                    await _botService.SendTextMessageAsync(
                    new MessageDto
                    {
                        Message = message,
                        Text = sb.ToString(),
                        CancellationToken = cancellationToken,
                    });
                }


                return subscription;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetSubscriptionList Error");
                throw new Exception($"GetSubscriptionList：{ex.Message}");
            }
        }

        private async Task<User?> GetUser(Message message, CancellationToken cancellationToken)
        {
            try
            {
                // 判斷使用者是否存在
                var user = await _context.Users.Where(x => x.Id == message.Chat.Id).FirstOrDefaultAsync(cancellationToken);
                if (user == null)
                {
                    user = new User
                    {
                        Id = (int)message.Chat.Id,
                        UserName = message.Chat.Username,
                    };
                    _context.Users.Add(user);
                    var count = await _context.SaveChangesAsync(cancellationToken);
                    if(count > 0)
                        return user;
                }

                // 使用者已被停用
                if (user.Status == true)
                    return null;

                return user;
            }
            catch (Exception ex)
            {
                {
                    _logger.LogError(ex, "CreateUser Error");
                    throw new Exception($"CreateUser：{ex.Message}");
                }
            }
        }

        private class StockData
        {
            public string stat { get; set; }
            public int total { get; set; }
        }
    }
}