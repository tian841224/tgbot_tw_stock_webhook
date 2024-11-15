using Telegram.Bot;
using TGBot_TW_Stock_Webhook.Dto;
using TGBot_TW_Stock_Webhook.Interface;
using TGBot_TW_Stock_Webhook.Services;
using TGBot_TW_Stock_Webhook.Services.Web;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


// Setup bot configuration
var botConfigSection = builder.Configuration.GetSection("BotConfiguration");
builder.Services.Configure<BotConfiguration>(botConfigSection);
builder.Services.AddHttpClient("tgwebhook").RemoveAllLoggers().AddTypedClient<ITelegramBotClient>(
    httpClient => new TelegramBotClient(botConfigSection.Get<BotConfiguration>()!.BotToken, httpClient));
builder.Services.AddScoped<UpdateHandler>();
builder.Services.AddSingleton<IBrowserHandlers, BrowserHandlers>();
builder.Services.AddScoped<IBotConfigurationService, BotConfigurationService>();
builder.Services.AddScoped<IBotService, BotService>();
builder.Services.AddScoped<ICommonService, CommonService>();
builder.Services.AddScoped<Cnyes>();
builder.Services.AddScoped<TradingView>();
builder.Services.AddScoped<Lazy<TradingView>>();
builder.Services.AddScoped<Lazy<Cnyes>>();

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
