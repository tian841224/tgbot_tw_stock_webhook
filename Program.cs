using Telegram.Bot;
using TGBot_TW_Stock_Webhook.Command;
using TGBot_TW_Stock_Webhook.Data;
using TGBot_TW_Stock_Webhook.Extensions;
using TGBot_TW_Stock_Webhook.Interface;
using TGBot_TW_Stock_Webhook.Interface.Repository;
using TGBot_TW_Stock_Webhook.Interface.Services;
using TGBot_TW_Stock_Webhook.Model;
using TGBot_TW_Stock_Webhook.Model.DTOs;
using TGBot_TW_Stock_Webhook.Repository;
using TGBot_TW_Stock_Webhook.Services;
using TGBot_TW_Stock_Webhook.Services.Bot;

var builder = WebApplication.CreateBuilder(args);

// HACK: 暫時性的解決方案
// UNDONE: 尚未完成的部分
// FIXME: 需要修復的問題

// Setup bot configuration
var botConfigSection = builder.Configuration.GetSection("BotConfiguration");
builder.Services.Configure<BotConfiguration>(botConfigSection);
builder.Services.AddHttpClient("tgwebhook").RemoveAllLoggers().AddTypedClient<ITelegramBotClient>(
    httpClient => new TelegramBotClient(botConfigSection.Get<BotConfiguration>()!.BotToken, httpClient));

// Add services to the container.
// Setting
var dbConfig = builder.Configuration.GetSection("DataBase").Get<DataBase>();
if (dbConfig == null)
{
    throw new InvalidOperationException("appsettings:Database configuration is missing.");
}
// 長時間執行的服務使用 Singleton
builder.Services.AddSingleton(dbConfig);
builder.Services.AddSingleton<IBotService, BotService>();

// 業務邏輯服務使用 Scoped
builder.Services.AddScoped<UpdateHandler>();
builder.Services.AddScoped<INotificationService, NotificationService>();

// 指令功能
builder.Services.AddScoped<ICommandFactory, CommandFactory>();
builder.Services.AddScoped<ICommand, KlineCommand>();
builder.Services.AddScoped<ICommand, DetailPriceCommand>();
builder.Services.AddScoped<ICommand, TopVolumeCommand>();
builder.Services.AddScoped<ICommand, PerformanceCommand>();
builder.Services.AddScoped<ICommand, NewsCommand>();
builder.Services.AddScoped<ICommand, ChartCommand>();
builder.Services.AddScoped<ICommand, DailyMarketInfoCommand>();
builder.Services.AddScoped<ICommand, AfterTradingVolumeCommand>();
builder.Services.AddScoped<ICommand, YahooNewsCommand>();
builder.Services.AddScoped<ICommand, SubscriptionStockCommand>();
builder.Services.AddScoped<ICommand, UnSubscriptionStockCommand>();
builder.Services.AddScoped<ICommand, GetSubscriptionStockListCommand>();
builder.Services.AddScoped<ICommand, SubscriptionInfoCommand>();
builder.Services.AddScoped<ICommand, UnSubscriptionInfoCommand>();

// Lazy延遲載入
builder.Services.AddLazyScoped<IBrowserHandlers, BrowserService>();
builder.Services.AddLazyScoped<ICommonService, CommonService>();
builder.Services.AddLazyScoped<Cnyes>();
builder.Services.AddLazyScoped<TradingView>();
builder.Services.AddLazyScoped<ISubscriptionService, SubscriptionService>();
builder.Services.AddLazyScoped<ITwStockService, TwStockService>();
builder.Services.AddLazyScoped<ITwStockBotService, TwStockBotService>();

// Repository
builder.Services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
builder.Services.AddScoped<ISubscriptionUserRepository, SubscriptionUserRepository>();
builder.Services.AddScoped<ISubscriptionUserStockRepository, SubscriptionUserStockRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// DB
builder.Services.AddDbContext<AppDbContext>();

builder.Services.ConfigureTelegramBotMvc();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddResponseCompression();
builder.Services.AddHealthChecks();
var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//}
app.UseSwagger();
app.UseSwaggerUI();

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapGet("/", () => "Hello World!");

app.Run();
