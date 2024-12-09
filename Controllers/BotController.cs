using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;
using TGBot_TW_Stock_Webhook.Infrastructure.Attribute;
using TGBot_TW_Stock_Webhook.Model.Configuration;
using TGBot_TW_Stock_Webhook.Services.Bot;

namespace TGBot_TW_Stock_Webhook.Controllers;

[ApiController]
[Route("[controller]")]
public class BotController(IOptions<BotConfiguration> Config) : ControllerBase
{
    [HttpPost("getWebhookInfo")]
    [HeaderValidationFilter]
    public async Task<IActionResult> GetWebhookInfo([FromServices] ITelegramBotClient bot, CancellationToken ct)
    {
        try
        {
            return Ok(await bot.GetWebhookInfo(ct));
        }
        catch (Exception exception)
        {
            return Problem(detail: exception.Message, statusCode: 500);
        }
    }

    [HttpPost("setWebhook")]
    [HeaderValidationFilter]
    public async Task<IActionResult> SetWebHook([FromBody] string token, [FromServices] ITelegramBotClient bot, CancellationToken ct)
    {
        try
        {
            // 若沒帶入 token 則使用設定檔中的 token
            var webhookUrl = token ?? Config.Value.BotWebhookUrl.AbsoluteUri;
            await bot.SetWebhook(webhookUrl, allowedUpdates: [], secretToken: Config.Value.SecretToken, cancellationToken: ct);
            return Ok($"Webhook set to {webhookUrl}");
        }
        catch (Exception exception)
        {
            return Problem(detail: exception.Message, statusCode: 500);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Update update, [FromServices] UpdateHandler handleUpdateService, CancellationToken ct)
    {
        try
        {
            if (Request.Headers["X-Telegram-Bot-Api-Secret-Token"] != Config.Value.SecretToken)
                return Forbid();

            await handleUpdateService.HandleUpdateAsync(update, ct);
            return Ok();
        }
        catch (Exception exception)
        {
            await handleUpdateService.HandleErrorAsync(exception, Telegram.Bot.Polling.HandleErrorSource.HandleUpdateError, ct);
            return Problem(detail: exception.Message, statusCode: 500);
        }

    }
}
