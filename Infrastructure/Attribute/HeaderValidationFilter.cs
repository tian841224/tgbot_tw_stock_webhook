using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace TGBot_TW_Stock_Webhook.Attribute
{
    public class HeaderValidationFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var headers = context.HttpContext.Request.Headers;

            // 檢查特定標頭是否存在且符合規範
            if (!headers.ContainsKey("ADMIN-Validation") ||
                !IsValidHeaderValue(headers["ADMIN-Validation"]))
            {
                context.Result = new UnauthorizedResult(); // 返回 401 未授權
                return;
            }
        }

        private bool IsValidHeaderValue(string headerValue)
        {
            // 自定義驗證邏輯
            return !string.IsNullOrEmpty(headerValue) &&
                   headerValue == "Secret";
        }
    }
}
