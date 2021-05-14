using Automation.Acts;
using Automation.JS;
using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Automation
{
    public class ActsManager
    {
        public Page Page;
        public ActsManager(Page pg)
        {
            Page = pg;
        }
        public enum acts
        {
            BrawlAtMH,
            OppInSpite,
            PeramPathVG
        }
        public async Task<bool> Do(acts a)
        {
            var c = new Candle(Page);
            var pnt = await c.Get();
            if (pnt.points < 1)
                return false;
            var trm = new TravelManager(Page);
            switch (a)
            {
                case acts.BrawlAtMH:
                    await BackOrOn();
                    await trm.GoTo(TravelManager.routes.MedusasHead);
                    //await Page.ReloadAsync();
                    var b = new BrawlAtMH(Page, this);
                    var g = true;
                    while (pnt.points > 0 && g)
                    {
                        (g, _) = await b.Post();
                        pnt = await c.Get();
                    }
                    //await BrawlAtMH.Perform(p);
                    break;
                case acts.OppInSpite:
                    //
                    //await OppInSpite.Perform(p);
                    break;
                case acts.PeramPathVG:
                    //
                    //await PeramPathVG.Perform(p);
                    break;
            }
            return true;
            return false;
        }
        public async Task<bool> BackOrOn()
        {
            var tries = 0;
            // go back until we hit the travel button and click it
            while (tries < 100)
            {
                tries++;
                Thread.Sleep(250);
                var ow = await Page.XPathAsync("//button[contains(text(), 'Onwards')]");
                if (ow.Length > 0 && ow?[0] != null)
                {
                    await ow[0].ClickAsync();
                    continue;
                }
                var pn = await Page.XPathAsync("//span[contains(., ' Perhaps not')]");
                if (pn.Length > 0 && pn?[0] != null)
                {
                    await pn[0].ClickAsync();
                    continue;
                }
                break;
            }
            return true;
        }
    }
}
