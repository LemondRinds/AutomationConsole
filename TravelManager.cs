using Automation.JS;
using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
            MedusasHead,
            WatchmakersHill
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
            return o;
        }

        public static async Task<bool> GoTo(routes r, Page p)
        {
            /*var trvlTasks = new Task<ElementHandle>[]
            {
                p.WaitForSelectorAsync("#main > div > div.storylets__welcome-and-travel > button"),
                p.WaitForSelectorAsync("#root > div > div:nth-child(2) > div:nth-child(4) > div:nth-child(1) > div > nav > ul > li:nth-child(2) > button > i")
            };
            var trvl = await Task.WhenAny(trvlTasks);
            await trvl.Result.ClickAsync();
            Thread.Sleep(500);
            var zo = await p.WaitForSelectorAsync("[alt='Zoom out']");
            for(var i = 0; i < 5; i++)
            {
                await zo.ClickAsync();
                Thread.Sleep(500);
            }
            switch (r)
            {
                case routes.MedusasHead:
                    await MedusaHead.GoTo(p);
                    break;
            }*/
            return true;
            return false;
        }
    }
}
