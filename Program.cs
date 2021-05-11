using PuppeteerSharp;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Automation
{
    class Program
    {
        static PuppeteerSharp.Browser browser;
        static PuppeteerSharp.LaunchOptions chromeOpts;
        static async Task<int> Main(string[] args)
        {
            try
            {
                if(args.Length == 0)
                {
                    Console.WriteLine(AppConfig.help);
                    Console.WriteLine(">> press enter to exit");
                    Console.ReadLine();
                    return 0;
                }
                var valid = AppConfig.ParseArgs(args);
                AppConfig.semaphore = new System.Threading.SemaphoreSlim(AppConfig.maxPara);
                if (valid == false)
                {
                    Console.WriteLine(">> press enter to exit");
                    Console.ReadLine();
                    return 0;
                }
                chromeOpts = await Browser.StartPuppeteer();
                await using var browser = await Puppeteer.LaunchAsync(chromeOpts);
                foreach (DeliciousFriend d in AppConfig.dfs)
                {
                    //var cntxt = await browser.CreateIncognitoBrowserContextAsync();
                    d.brwsr = browser.DefaultContext;
                    if(await d.Login())
                    {
                        await d.Do();
                    }
                }
                //await Task.WhenAll(AppConfig.allTasks);
                //AppConfig.WriteOut($">> null page responses: {AppConfig.NullResponse}\tpage response handler errors: {AppConfig.HandledErrs}");
                AppConfig.WriteOut(">> press enter to exit", true);
                if (AppConfig.silent)
                {

                }
                return 1;
            }
            catch (Exception ex)
            {
                AppConfig.ErrHand(ex);
                AppConfig.WriteOut(">> press enter to exit", true);
                return 0;
            }
            finally
            {
                if(browser != null)
                    await browser.DisposeAsync();
                if (AppConfig.ol != null)
                {
                    AppConfig.ol.Close();
                    AppConfig.ol.Dispose();
                }
            }
        }
    }
}
