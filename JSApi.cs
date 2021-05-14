using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Automation
{
    public class JSApi
    {
        public static string hGet(string pth, string success, string fail)
        {
            return "var xmlHttp = new XMLHttpRequest();" +
                $"xmlHttp.open('GET', 'https://api.{AppConfig.url}{pth}/api', false);" +
                $"xmlHttp.setRequestHeader('Authorization', 'Bearer {AppConfig.sessId}');" +
                "xmlHttp.send(null);" +
                "if(xmlHttp.status == 200){" +
                    "try{" +
                        $"{success}" +
                    "}catch{" +
                        $"{fail}" +
                    "}" +
                "}else{" +
                    $"{fail}" +
                "}";
        }
        public static string hPost(string pth, string success, string fail, string body = null)
        {
            return "var xmlHttp = new XMLHttpRequest();" +
                $"xmlHttp.open('POST', 'https://api.{AppConfig.url}{pth}/api', false);" +
                $"xmlHttp.setRequestHeader('Authorization', 'Bearer {AppConfig.sessId}');" +
                "xmlHttp.setRequestHeader('content-type','application/json');" +
                $"xmlHttp.send({(string.IsNullOrEmpty(body) ? "null" : "JSON.stringify(" + body + ")")});" +
                "if(xmlHttp.status == 200){" +
                    "try{" +
                        $"{success}" +
                    "}catch{" +
                        $"{fail}" +
                    "}" +
                "}else{" +
                    $"{fail}" +
                "}";
        }
    }
}
