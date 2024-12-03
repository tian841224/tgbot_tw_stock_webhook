using PuppeteerSharp;

namespace TGBot_TW_Stock_Webhook.Interface.Services
{
    public interface IBrowserHandlers
    {
        /// <summary>釋放瀏覽器</summary>
        Task ReleaseBrowser();

        /// <summary>載入網頁</summary>
        Task<IPage> LoadUrlAsync(string url);

        /// <summary>關閉葉面</summary>
        Task ClosePage();
    }
}