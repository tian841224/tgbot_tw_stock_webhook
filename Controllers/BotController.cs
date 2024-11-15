using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;
using TGBot_TW_Stock_Webhook.Dto;
using TGBot_TW_Stock_Webhook.Services;

namespace TGBot_TW_Stock_Webhook.Controllers;

[ApiController]
[Route("[controller]")]
public class BotController(IOptions<BotConfiguration> Config) : ControllerBase
{
    [HttpPost("setWebhook")]
    public async Task<string> SetWebHook([FromBody] string token, [FromServices] ITelegramBotClient bot, CancellationToken ct)
    {
        // 若沒帶入 token 則使用設定檔中的 token
        var webhookUrl = token ?? Config.Value.BotWebhookUrl.AbsoluteUri;
        await bot.SetWebhook(webhookUrl, allowedUpdates: [], secretToken: Config.Value.SecretToken, cancellationToken: ct);
        return $"Webhook set to {webhookUrl}";
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Update update, [FromServices] UpdateHandler handleUpdateService, CancellationToken ct)
    {
        if (Request.Headers["X-Telegram-Bot-Api-Secret-Token"] != Config.Value.SecretToken)
            return Forbid();
        try
        {
            await handleUpdateService.HandleUpdateAsync(update, ct);
        }
        catch (Exception exception)
        {
            await handleUpdateService.HandleErrorAsync(exception, Telegram.Bot.Polling.HandleErrorSource.HandleUpdateError, ct);
        }
        return Ok();
    }
}
