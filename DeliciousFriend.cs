using Automation.JS;
using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Automation
{
    public class DeliciousFriend
    {
        public BrowserContext brwsr;
        private ActsManager.acts act;
        private string un;
        private string pw;
        public DeliciousFriend(ActsManager.acts a, string u, string p)
        {
            act = a;
            un = u;
            pw = p;
        }
        public async Task<bool> Login()
        {
            var pages = await brwsr.Browser.DefaultContext.PagesAsync();
            var page = pages[0];
            await Browser.Login(page, un, pw);//brwsr, un, pw);
            return true;
        }
        public async Task<bool> Do()
        {
            var pages = await brwsr.PagesAsync();
            var p = pages[0];
            var am = new ActsManager(p);
            await am.Do(act);
            return true;
        }
    }
}
