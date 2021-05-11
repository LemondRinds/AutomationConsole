using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Automation.JS
{
    public class Candle : AbsJS
    { 
        // make singleton??
        public override Page Page { get; set; }
        public Candle(Page pg){
            Page = pg;
        }
        public class JSApiCandlePointsResponse
        {
            public bool success;
            public int points;
        }
        public override string Path { get { return "/character/actions"; } }
        public override string GetSuccess { get { return "return {success:true, points:JSON.parse(xmlHttp.response).actions};"; } }
        public override string GetFail { get { return "return {success:false};"; } }
        private string FnString()
        {
            return "(function (){" +
                $"{JSApi.hGet(Path,GetSuccess,GetFail)}" +
            "})()";
        }
        public async Task<JSApiCandlePointsResponse> Get()
        {
            var o = await Page.EvaluateFunctionAsync<JSApiCandlePointsResponse>($"() => {FnString()}");
            return o;
        }
    }
}
