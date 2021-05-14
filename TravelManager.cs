using Automation.JS;
using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace Automation
{
    public class TravelManager : AbsJS
    {
        public TravelManager(Page pg)
        {
            Page = pg;
        }
        public enum routes
        {
            WatchmakersHill,
            MedusasHead,
            DeptOfMenaceErad,
            VeilGarden,
            TheSingingMandrake,
            LadybonesRoad,
            MolochStreet,
            Spite,
            AreaDiving,
            Crowds,
            Flit,
            Carnival,
            ForgottenQquarter,
            BaseCamp,
            TempleClub
        }
        public class JSApiTravelResponse
        {
            public bool success;
        }
        public override Page Page { get; set; }
        public override string Path { get { return "/map/move"; } }
        public override string GetSuccess { get { return 
            "var rs = JSON.parse(xmlHttp.response);" +
            "if(rs.message == 'You are already in this area' || rs.isSuccess){" + 
                "return {success:true}" +
            "}else{" +
                $"{GetFail}" +
            "}"; } }
        public override string GetFail { get { return "return {success:false};"; } }
        private string FnString(int a)
        {
            return "(function (){" +
                $"{JSApi.hPost(Path, GetSuccess, GetFail, $"{{'areaId':{a}}}")}" +
            "})()";
        }
        public async Task<JSApiTravelResponse> Post(routes r)
        {
            int areaId = 0;
            switch (r)
            {
                case routes.MedusasHead:
                    areaId = 110082;
                    break;
                case routes.WatchmakersHill:
                    areaId = 5;
                    break;
            }
            var o = await Page.EvaluateFunctionAsync<JSApiTravelResponse>($"() => {FnString(areaId)}");
            await Page.ReloadAsync();
            return o;
        }

        public async Task<bool> GoTo(routes r)
        {
            string where = "";
            switch (r)
            {
                case routes.MedusasHead:
                    where = "The Medusa's Head";
                    break;
                case routes.WatchmakersHill:
                    where = "Watchmaker's Hill";
                    break;
                case routes.DeptOfMenaceErad:
                    where = "Dept. of Menace Eradication";
                    break;
                case routes.VeilGarden:
                    where = "Veilgarden";
                    break;
                case routes.TheSingingMandrake:
                    where = "The Singing Mandrake";
                    break;
                case routes.Spite:
                    where = "Spite";
                    break;
            }
            await TravelButton();
            var tries = 0;
            while (tries < 100)
            {
                tries++;
                Thread.Sleep(250);
                var map = await Page.QuerySelectorAsync(".ReactModal__Content.ReactModal__Content--after-open.modal--map__content");
                if (map != null)
                {
                    var zo = await Page.QuerySelectorAsync("[alt='Zoom out'].leaflet-control--custom-zoom--disabled");
                    var zi = await Page.QuerySelectorAsync("[alt='Zoom in']");
                    if (zo != null && zi != null)
                    {
                        await zi.ClickAsync();
                        continue;
                    }
                    if (zo == null && zi != null)
                    {
                        var mh = await Page.XPathAsync($"//div[div[div[contains(text(), \"{where}\")]]]");
                        if (mh.Length > 0 && mh?[0] != null)
                        {
                            await mh[0].ClickAsync();
                            continue;
                        }
                    }
                }
                else
                {
                    var trvldMess = await Page.XPathAsync($"//div[contains(@class,'storylets__welcome-and-travel')]//div[contains(.,\"{where}\")]");
                    if (trvldMess != null)
                    {
                        break;
                    }
                }
            }
            return true;
            return false;
        }
        public async Task<bool> TravelButton()
        {
            var tries = 0;
            while (tries < 100)
            {
                tries++;
                Thread.Sleep(250);
                var trvl = await Page.QuerySelectorAsync("div.storylets__welcome-and-travel > button");
                if (trvl == null)
                    trvl = await Page.QuerySelectorAsync("button[title='Map']");
                if (trvl == null)
                    continue;
                await trvl.ClickAsync();
                break;
            }
            return true;
        } 
    }
}
