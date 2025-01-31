using PuppeteerSharp;
using TGBot_TW_Stock_Webhook.Interface.Services;

namespace TGBot_TW_Stock_Webhook.Services
{
    public class BrowserService(ILogger<BrowserService> _logger) : IBrowserHandlers
    {
        private IBrowser? _browser = null;
        private IPage? _page = null;
        private readonly TimeSpan _timeout = TimeSpan.FromMinutes(1);

        /// <summary>
        /// 載入網頁
        /// </summary>
        /// <param name="stockNumber"></param>
        /// <returns></returns>
        public async Task<IPage> LoadUrlAsync(string url)
        {
            try
            {
                await InitAsync();

                if (_page is null)
                {
                    throw new InvalidOperationException("頁面未正確初始化");
                }

                await _page.GoToAsync(url, new NavigationOptions
                {
                    Timeout = (int)_timeout.TotalMilliseconds,
                    WaitUntil =
                    [
                        WaitUntilNavigation.DOMContentLoaded
                    ]
                });

                _logger.LogInformation("等待元素載入...");
                return _page;
            }
            catch (Exception ex)
            {
                await ReleaseBrowserAsync();
                _logger.LogError(ex.Message, "LoadUrlAsync");
                throw new Exception($"LoadUrlAsync : {ex.Message}");
            }
        }

        /// <summary>
        /// 關閉頁面
        /// </summary>
        /// <returns></returns>
        public async Task ClosePageAsync()
        {
            try
            {
                if (_page != null)
                    await _page.CloseAsync();
                _page = null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "ClosePage");
                throw new Exception($"ClosePage: {ex.Message}");

            }
        }

        /// <summary>
        /// 釋放瀏覽器流程
        /// </summary>
        /// <returns></returns>
        public async Task ReleaseBrowserAsync()
        {
            try
            {
                await ClosePageAsync();
                await CloseBrowserAsync();
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 初始化瀏覽器及頁面
        /// </summary>
        /// <returns></returns>
        private async Task InitAsync()
        {
            try
            {
                await ReleaseBrowserAsync();
                if (_browser == null)
                    await LaunchBrowserAsync();

                if (_browser == null)
                    throw new InvalidOperationException("初始化Browser錯誤");

                if (_page == null)
                {
                    _page = await _browser.NewPageAsync();
                    await _page.SetViewportAsync(new ViewPortOptions
                    {
                        Width = 1920,
                        Height = 1080
                    });
                    _logger.LogInformation("啟動頁面");
                }

                if (_page == null)
                    throw new InvalidOperationException("初始化Page錯誤");
            }
            catch (Exception ex)
            {
                await ReleaseBrowserAsync();
                _logger.LogError(ex.Message, "InitAsync");
                throw new Exception($"InitAsync:{ex.Message}");
            }
        }

        /// <summary>
        /// 啟動瀏覽器
        /// </summary>
        /// <returns></returns>
        private async Task LaunchBrowserAsync()
        {
            try
            {
                var options = new LaunchOptions
                {
                    Args = [
                            "--disable-dev-shm-usage",
                            "--disable-setuid-sandbox",
                            "--no-sandbox",
                            "--disable-gpu",
                        ],
                    Headless = true,
                };

                /*
                若為Win系統使用GetInstalledBrowsers判斷是否有瀏覽器
                Liunx系統Docker內已安裝，直接執行即可
                */
                // 判斷作業系統
                if (Environment.OSVersion.ToString().Contains("Windows"))
                {
                    // 檢查瀏覽器是否已經存在
                    var browserFetcher = new BrowserFetcher();
                    if (browserFetcher.GetInstalledBrowsers().Count() == 0)
                    {
                        _logger.LogInformation("瀏覽器不存在，正在下載...");
                        await browserFetcher.DownloadAsync();
                        _logger.LogInformation("瀏覽器下載完成");
                    }
                }
                else
                {
                    options.ExecutablePath = "/usr/bin/google-chrome-stable";
                }

                // 啟動瀏覽器
                _browser = await Puppeteer.LaunchAsync(options);

                _logger.LogInformation("啟動瀏覽器");
            }
            catch (Exception ex)
            {
                await ReleaseBrowserAsync();
                _logger.LogError(ex.Message, "LaunchBrowserAsync");
                throw;
            }
        }

        /// <summary>
        /// 關閉瀏覽器
        /// </summary>
        /// <returns></returns>
        private async Task CloseBrowserAsync()
        {
            try
            {
                if (_browser != null)
                    await _browser.DisposeAsync();

                _browser = null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "CloseBrowser");
                throw new Exception($"CloseBrowser: {ex.Message}");
            }
        }
    }
}
