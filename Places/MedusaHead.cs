using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Automation.Places
{
    public class MedusaHead// : IPlace
    {
        public static int id = 5;
        public async Task<bool> GoTo(Page p)
        {
            /*Thread.Sleep(500);
            //await p.WaitForXPathAsync("//div[contains(string(), \"Watchmaker's Hill\")]");
            //var pl = await p.XPathAsync("//div[contains(string(), \"Watchmaker's Hill\")]");
            await p.WaitForSelectorAsync(".leaflet-tooltip.leaflet-tooltip--fbg.leaflet-tooltip--fbg-interactable.leaflet-zoom-animated.leaflet-tooltip-center div");
            var pl = await p.QuerySelectorAllAsync(".leaflet-tooltip.leaflet-tooltip--fbg.leaflet-tooltip--fbg-interactable.leaflet-zoom-animated.leaflet-tooltip-center div");
            foreach(ElementHandle e in pl)
            {
                //await e.html
            }
            await pl[7].ClickAsync();
            Thread.Sleep(500);
            await p.WaitForXPathAsync("//div[contains(string(), \"Medusa's Head\")]");
            pl = await p.XPathAsync("//div[contains(string(), \"Medusa's Head\")]");
            await pl[7].ClickAsync();*/

            return true;
            return false;
        }
    }
}
