using Telegram.Bot;
using TGBot_TW_Stock_Webhook.Data;
using TGBot_TW_Stock_Webhook.Dto;
using TGBot_TW_Stock_Webhook.Extensions;
using TGBot_TW_Stock_Webhook.Interface;
using TGBot_TW_Stock_Webhook.Model;
using TGBot_TW_Stock_Webhook.Services;
using TGBot_TW_Stock_Webhook.Services.Web;

var builder = WebApplication.CreateBuilder(args);



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
builder.Services.AddSingleton<UpdateHandler>();

// 業務邏輯服務使用 Scoped
// builder.Services.AddScoped<IBotConfigurationService, BotConfigurationService>();

// Lazy延遲載入
builder.Services.AddLazyScoped<IBrowserHandlers, BrowserHandlers>();
builder.Services.AddLazyScoped<ICommonService, CommonService>();
builder.Services.AddLazyScoped<Cnyes>();
builder.Services.AddLazyScoped<TradingView>();

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
