using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Text.Json;
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
                var subscription = await _context.Subscriptions.FirstOrDefaultAsync(x => x.UserId == user.Id && x.Symbol == stock && x.IsDelete == false, cancellationToken);
                string companyName = string.Empty;
                if (subscription == null)
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
                    {
                        await _botService.SendTextMessageAsync(new MessageDto
                        {
                            Message = message,
                            Text = $"訂閱失敗：{stock}，股票代碼錯誤",
                            CancellationToken = cancellationToken,
                        });
                        throw new Exception($"訂閱失敗：{stock}，股票代碼錯誤");
                    }

                    // 查詢股票名稱
                    url = $"https://www.twse.com.tw/rwd/zh/api/codeQuery?query={stock}";
                    response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    // 解析 JSON
                    var json = JObject.Parse(await response.Content.ReadAsStringAsync());
                    string firstSuggestion = json["suggestions"][0].ToString();
                    // 取出公司名
                    companyName = firstSuggestion.Split('\t')[1];

                    //取得當前使用者訂閱清單
                    var userSubList = await GetSubscriptionList(user, cancellationToken);

                    // 新增訂閱
                    subscription = new Subscription
                    {
                        UserId = user.Id,
                        Serial = (uint)userSubList.Count() + 1,
                        Symbol = stock,
                        Name = companyName,
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
                        Text = $"訂閱成功：{stock}/{companyName}",
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
                var subscription = await _context.Subscriptions.FirstOrDefaultAsync(x => x.UserId == user.Id && x.Symbol == stock,cancellationToken);
                if (subscription == null) return;

                // 刪除訂閱
                subscription.IsDelete = true;
                _context.Subscriptions.Update(subscription);

                // 訂閱清單重新編號
                var userSubList = await GetSubscriptionList(user, cancellationToken);
                uint serial = 1;
                foreach (var item in userSubList)
                {
                    item.Serial = serial++;
                    _context.Subscriptions.Update(item);
                }

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
                    subscription.ForEach(item => sb.AppendLine($"{item.Serial}：{item.Symbol}/{item.Name}"));

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

        private async Task<List<Subscription>> GetSubscriptionList(User user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                // 判斷是否已經訂閱過
                var subscription = await _context.Subscriptions.Where(x => x.UserId == user.Id && x.IsDelete == false).ToListAsync(cancellationToken);
                if(subscription == null) return new List<Subscription>();
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
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == message.Chat.Id,cancellationToken);
                if (user == null)
                {
                    user = new User
                    {
                        Id = (int)message.Chat.Id,
                        UserName = message.Chat.Username,
                    };
                    _context.Users.Add(user);
                    var count = await _context.SaveChangesAsync(cancellationToken);
                    if (count > 0)
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
            public string stat { get; set; } = string.Empty;
            public int total { get; set; }
        }
    }
}