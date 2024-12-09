using Microsoft.AspNetCore.Mvc;
using TGBot_TW_Stock_Webhook.Interface.Services;

namespace TGBot_TW_Stock_Webhook.Controllers
{
    [ApiController]
    [Route("[controller]/[Action]")]
    public class NotificationController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> SendDailyMarketInfoAsync([FromServices] INotificationService notificationService, CancellationToken ct)
        {
            try
            {
                await notificationService.SendDailyMarketInfoAsync(ct);
                return Ok();
            }
            catch (Exception exception)
            {
                return new BadRequestObjectResult(exception);
            }
        }

        [HttpGet]
        public async Task<IActionResult> SendStockInfoAsync([FromServices] INotificationService notificationService, CancellationToken ct)
        {
            try
            {
                await notificationService.SendStockInfoAsync(ct);
                return Ok();
            }
            catch (Exception exception)
            {
                return new BadRequestObjectResult(exception);
            }
        }

        [HttpGet]
        public async Task<IActionResult> SendStockNewsAsync([FromServices] INotificationService notificationService, CancellationToken ct)
        {
            try
            {
                await notificationService.SendStockNewsAsync(ct);
                return Ok();
            }
            catch (Exception exception)
            {
                return new BadRequestObjectResult(exception);
            }
        }

        [HttpGet]
        public async Task<IActionResult> SendTopVolumeItemsAsync([FromServices] INotificationService notificationService, CancellationToken ct)
        {
            try
            {
                await notificationService.SendTopVolumeItemsAsync(ct);
                return Ok();
            }
            catch (Exception exception)
            {
                return new BadRequestObjectResult(exception);
            }
        }
    }
}
