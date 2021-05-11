using Automation.Acts;
using Automation.JS;
using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Automation
{
    public static class ActsManager
    {
        public enum acts
        {
            BrawlAtMH,
            OppInSpite,
            PeramPathVG
        }
        public static async Task<bool> Do(acts a, Page p)
        {
            var c = await new Candle(p).Get();
            if (c.points < 1)
                return false;
            var trm = new TravelManager(p);
            switch (a)
            {
                case acts.BrawlAtMH:
                    await trm.Post(TravelManager.routes.MedusasHead);
                    var b = new BrawlAtMH(p);
                    var cp = c.points;
                    var g = true;
                    while (cp > 0 && g)
                    {
                        (g, cp) = await b.Post();
                    }
                    await b.Post();
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
    }
}
