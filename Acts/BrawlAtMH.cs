using Automation.JS;
using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Automation.Acts
{
    public class BrawlAtMH : AbsJS
    {
        public BrawlAtMH(Page pg, ActsManager am)
        {
            Page = pg;
            this.am = am;
        }
        public class JSApiBeginResponse
        {
            public bool success;
            public JSApiEventStorylet storylet;
            public string message;
            public string phase;
            public int actions;
        }
        public class JSApiChildBranch
        {
            public int id;
            public string name;
            public bool isLocked;
        }
        public class JSApiEventStorylet
        {
            public int id;
            public bool canGoBack;
            public string name;
            public JSApiChildBranch[] childBranches;
        }
        public class JSApiChildBranchResponse
        {
            public bool success;
            public string message;
            public int id;
            public string phase;
        }
        public class JSApiEndResponse
        {
            public int actions;
            public bool isSuccess;
        }
        public override Page Page { get; set; }
        public ActsManager am;
        public override string Path { get { return "/storylet/begin"; } }
        public string beginPath = "/storylet/begin";
        public string choicePath = "/storylet/choosebranch";
        public string endPath = "/storylet";
        public override string GetSuccess
        {
            get
            {
                return
                "var rs = JSON.parse(xmlHttp.response);" +
                "if(rs.isSuccess){" +
                    "return {success:true, storylet:rs.storylet, message:rs.message, phase:rs.phase, actions:rs.actions}" +
                "}else{" +
                    $"{GetFail}" +
                "}";
            }
        }
        public string GetChoiceSuccess
        {
            get
            {
                return
                "var rs = JSON.parse(xmlHttp.response);" +
                "if(rs.isSuccess){" +
                    "return {success:true, id:rs.endStorylet.event.id, message:rs.message, phase:rs.phase}" +
                "}else{" +
                    $"{GetFail}" +
                "}";
            }
        }
        public string GetEndSuccess
        {
            get
            {
                return
                "var rs = JSON.parse(xmlHttp.response);" +
                "if(rs.isSuccess){" +
                    "return rs;" +
                "}else{" +
                    "rs.isSuccess = false;" +
                    "return rs;" +
                "}";
            }
        }
        public override string GetFail { get { return "return {success:false};"; } }
        private string Begin()
        {
            return "(function (){" +
                $"{JSApi.hPost(beginPath, GetSuccess, GetFail, "{'eventId':321522}")}" +
            "})()";
        }
        private string BranchId(int i)
        {
            return "(function (){" +
                $"{JSApi.hPost(choicePath, GetChoiceSuccess, GetFail, $"{{'branchId':{i}}}")}" +
            "})()";
        }
        private string Storylet()
        {
            return "(function (){" +
                $"{JSApi.hPost(endPath, GetSuccess, GetFail, "")}" +
            "})()";
        }
        private string End()
        {
            return "(function (){" +
                $"{JSApi.hPost(endPath, GetEndSuccess, GetFail, "")}" +
            "})()";
        }
        public async Task<(bool,int)> Post()
        {
            var tries = 0;
            while(tries < 100)
            {
                tries++;
                Thread.Sleep(250);
                var exit = await Page.QuerySelectorAsync("div.buttons.buttons--storylet-exit-options button");
                if(exit != null)
                {
                    await exit.ClickAsync();
                    break;
                }
                var ow = await Page.XPathAsync("//button[contains(text(), 'Onwards')]");
                if (ow.Length > 0 && ow?[0] != null)
                {
                    await ow[0].ClickAsync();
                    continue;
                }
                var brwl = await Page.QuerySelectorAsync("div[data-branch-id='321522'] button.button--margin.button--go");
                if (brwl != null)
                {
                    await brwl.ClickAsync();
                    continue;
                }
                var tkCmrs = await Page.QuerySelectorAsync("div[data-branch-id='236494'] button.button--margin.button--go");
                if (tkCmrs != null)
                {
                    await tkCmrs.ClickAsync();
                    continue;
                }
                var doUnwrap = await Page.XPathAsync("//h1[contains(text(), \"What's in the bundle?\")]");
                if(doUnwrap != null)
                {
                    var go = await Page.QuerySelectorAsync(".button--margin.button--go");
                    await go.ClickAsync();
                    continue;
                }
            }
            /*// begin
            var o = await Page.EvaluateFunctionAsync<JSApiBeginResponse>($"() => {Begin()}");
            await Page.ReloadAsync();
            if (!o.success || o?.storylet?.childBranches?.Length == null || o.phase != "In")
            {
                //if (o.storylet.canGoBack)
                    //try to go back
                return (false,0);
            }
            // maybe some prefer logic for branches here? check choices?
            var ci = o.storylet.childBranches?[o.storylet.childBranches.Length - 1]?.id;
            if (ci == null)
                return (false, 0);
            // choice
            var oo = await Page.EvaluateFunctionAsync<JSApiChildBranchResponse>($"() => {BranchId((int)ci)}");
            await Page.ReloadAsync();
            if (!oo.success || oo.id < 1)
                return (false, 0);
            // storylet
            var ooo = await Page.EvaluateFunctionAsync<JSApiBeginResponse>($"() => {Storylet()}");
            await Page.ReloadAsync();
            if (!ooo.success || o?.storylet?.childBranches?.Length == null)
                return (false, 0);
            if (ooo.phase == "End")
                return (true, ooo.actions);
            var cii = ooo.storylet.childBranches?[ooo.storylet.childBranches.Length - 1]?.id;
            if (cii == null)
                return (false, 0);
            // choice
            var oooo = await Page.EvaluateFunctionAsync<JSApiChildBranchResponse>($"() => {BranchId((int)cii)}");
            await Page.ReloadAsync();
            if (!oooo.success || oooo.id < 1)
                return (false, 0);
            var ooooo = await Page.EvaluateFunctionAsync<JSApiEndResponse>($"() => {End()}");
            await Page.ReloadAsync();
            if (!ooooo.isSuccess)
                return (false, 0);*/
            return (true,0);
        }

    }
}
