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
            LaunchOptions chromeOpts = new LaunchOptions {
                Headless = false,
                Args = new string[]
                {
                    "--disable-web-security",
                    "--disable-features=IsolateOrigins,site-per-process",
                    "--disable-features=IsolateOrigins",
                    "--disable-site-isolation-trials",
                    "--disable-oor-cors"
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
        public static async Task<bool> Login(Page page, string u, string p){//BrowserContext browser, string u, string p) {
            try
            {
                //var page = await browser.NewPageAsync();
                await page.GoToAsync($"http://{AppConfig.url}/login");
                ElementHandle lgn;
                if (!string.IsNullOrEmpty(u))
                {
                    var un = await page.WaitForSelectorAsync(AppConfig.unFld);
                    var pw = await page.WaitForSelectorAsync(AppConfig.pwFld);
                    lgn = await page.WaitForSelectorAsync(AppConfig.lgnBtn);
                    await un.TypeAsync(u);
                    await pw.TypeAsync(p);
                }
                else
                {
                    lgn = await page.WaitForSelectorAsync(".button.button--google");
                }
                await lgn.ClickAsync();
                await page.WaitForNavigationAsync();
                //var cks = await page.GetCookiesAsync();
                //AppConfig.cks = cks.ToList();//.Where(c => c.Domain == new Uri(page.Url).Host).ToList();
                //AppConfig.sessId = AppConfig.cks.Where(c => c.Name == AppConfig.sessIdName).FirstOrDefault()?.Value;
                //AppConfig.WriteOut($">> session id set to {AppConfig.sessId}");
                AppConfig.sessId = await page.EvaluateFunctionAsync<string>("() => window.sessionStorage.access_token");
                return true;
            }
            catch(NavigationException ex)
            {
                AppConfig.ErrHand(ex, $"XX failed to navigate to apiurl");
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
