using PuppeteerSharp;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Automation
{
    public static class Browser
    {
        public static async Task<LaunchOptions> StartPuppeteer()
        {
            var cctPth = Path.Join(".\\WaveExtension", "3.1.3_0");
            cctPth = Path.GetFullPath(cctPth);
            LaunchOptions chromeOpts = new LaunchOptions {
                Headless = false,
                Args = new string[]
                {
                    $"--load-extension={cctPth}",
                    $"--disable-extensions-except={cctPth}",
                    "--disable-web-security",
                    "--disable-features=IsolateOrigins,site-per-process",
                    "--disable-features=IsolateOrigins",
                    "--disable-site-isolation-trials",
                    "--disable-oor-cors",
                    "--unsafely-treat-insecure-origin-as-secure=chrome-extension://jbbplnpkjmmeebjpijfedlgcdilocofh/sidebar.html"
                }
            };
            AppConfig.WriteOut(">> downloading chrome if not found");
            var dlTask = new BrowserFetcher().DownloadAsync();
            while(dlTask.IsCompleted != true)
            {
                if(!AppConfig.silent)
                    AppConfig.ConsoleSpinner();
            }
            AppConfig.WriteOut(">> done");
            //await using var browser = await Puppeteer.LaunchAsync(chromeOpts);
            return chromeOpts;
        }
        public static async Task<bool> LoginToTiqa(PuppeteerSharp.Browser browser) {
            try
            {
                var pages = await browser.PagesAsync();
                var page = pages[0];
                await page.GoToAsync(AppConfig.tiqaUrl);
                var un = await page.WaitForSelectorAsync(AppConfig.unFld);
                var pw = await page.WaitForSelectorAsync(AppConfig.pwFld);
                var lgn = await page.WaitForSelectorAsync(AppConfig.lgnBtn);
                await un.TypeAsync(AppConfig.un);
                await pw.TypeAsync(AppConfig.pw);
                await lgn.ClickAsync();
                await page.WaitForNavigationAsync();
                var cks = await page.GetCookiesAsync();
                AppConfig.cks = cks.ToList();//.Where(c => c.Domain == new Uri(page.Url).Host).ToList();
                AppConfig.sessId = AppConfig.cks.Where(c => c.Name == AppConfig.sessIdName).FirstOrDefault().Value;
                AppConfig.WriteOut($">> session id for crawl set to {AppConfig.sessId}");
                AppConfig.crawlStartUri = page.Url;
                return true;
            }
            catch(NavigationException ex)
            {
                AppConfig.ErrHand(ex, $"XX failed to navigate to url");
                return false;
            }
            catch (Exception ex)
            {
                AppConfig.ErrHand(ex);
                return false;
            }
        }
    }
}
