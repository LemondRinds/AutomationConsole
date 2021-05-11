using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;

namespace Automation
{
    public static class AppConfig
    {
        public static string url = "https://api.fallenlondon.com/api";
        public static readonly string EOL = Environment.NewLine;
        public static string sessId;
        public static string unFld;
        public static string pwFld;
        public static string lgnBtn;
        public static string un;
        public static string pw;
        public static List<DeliciousFriend> dfs = new List<DeliciousFriend>();
        public static string sessIdName = "ASP.NET_SessionId";
        public static List<CookieParam> cks = new List<CookieParam>();
        public static List<Task> allTasks = new List<Task>();
        public static List<BrowserContext> contexts = new List<BrowserContext>();
        public static string act;
        public static string crawlStartUri;
        public static int maxPara = 4;
        public static SemaphoreSlim semaphore;// = new SemaphoreSlim(maxPara);
        public static int NullResponse = 0;
        public static int HandledErrs = 0;
        public static bool silent = false;
        public static bool itrpt = false;
        public static bool noExcep = false;
        public static bool shortCirc = false;
        public static string outLog = Path.GetFullPath(Path.Combine("", "out.log"));
        public static bool log = false;
        public static StreamWriter ol;
        public static readonly string help = $"AUTOMATION <<{EOL}" +
            $">> uses chrome and puppeteer to automate stuff online{EOL}{EOL}" +
            $"OUTPUTS <<{EOL}" +
            $">> none so far{EOL}{EOL}" +
            $"INPUTS <<{EOL}" +
            $">> {"switch",-20}             {"required",-10}    {"default",-30}                     {"info",-10}{EOL}" +
            $">> {"/u username",-20}        {"X",-10}           {"",-30}                            username for login to whatever, can be comma delimited for many{EOL}" +
            $">> {"/uf usernameField",-20}  {"X",-10}            {"[name='']",-30}                   input name for username data{EOL}" +
            $">> {"/p password",-20}        {"X",-10}           {"",-30}                            password for login to whatever, can be comma delimited for many{EOL}" +
            $">> {"/pf passwordField",-20}  {"X",-10}            {"[name='']",-30}                   input name for password data{EOL}" +
            $">> {"/lf loginField",-20}     {"X",-10}            {"[name='']",-30}                   input name for submitting login data. Blank /u and /pw will trigger google+ login{EOL}" +
            $">> {"/a act",-20}             {"X",-10}           {"",-30}                            act for a delicious friend to take, can be comma delimited for many{EOL}" +
            $">> {"/s",-20}                 {"",-10}            { "",-30}                           only outputs exceptions and the wave error count{EOL}" +
            //$">> {"/t threads",-20}         {"",-10}            {"4",-30}                           max concurrent crawler/chrome requests{EOL}" +
            $">> {"/ne",-20}                {"",-10}            {"",-30}                            no handled exception output{EOL}" +
            $">> {"/l",-20}                 {"",-10}            {"",-30}                            writes to logs, respects /ne and /s{EOL}" +
            $">> {"/o outLog",-20}          {"",-10}            {"./out.log",-30}                   file to write to{EOL}{EOL}" +
            $"TO DO/FUTURE <<{EOL}" +
            $">> nothing yet";
        public static void ErrHand(Exception ex, string msg = null)
        {
            if (!noExcep || ex != null)
            {
                string otpt = msg ?? "XX an unhandled exception occurred";
                if (ex != null)
                    otpt += EOL + "XX " + (ex.Message.Length > 0 ? ex.Message : "") + (ex.StackTrace.Length > 0 && string.IsNullOrEmpty(msg) ? EOL + ex.StackTrace : "");
                Console.WriteLine(otpt);
                if (log)
                    ol.WriteLine(otpt);
            }
        }
        public static void WriteOut(string msg, bool rdIn = false, bool toLog = true)
        {
            if (!silent)
            {
                Console.WriteLine(msg);
                if (log && toLog)
                    ol.WriteLine(msg);
                if (rdIn)
                    Console.ReadLine();
            }
        }
        public static int counter;
        public static void ConsoleSpinner()
        {
            counter++;
            switch (counter % 4)
            {
                case 0: Console.Write("/"); counter = 0; break;
                case 1: Console.Write("-"); break;
                case 2: Console.Write("\\"); break;
                case 3: Console.Write("|"); break;
            }
            Thread.Sleep(100);
            Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
        }
        public static bool ParseArgs(string[] args)
        {
            var argList = new List<string>(args);
            bool res = true;
            string[] uns = new string[0];
            string[] pws = new string[0];
            int[] acts = new int[0];
            int posOfSwitch = argList.IndexOf("/u");
            if (posOfSwitch == -1)
            {
                Console.WriteLine("XX need a username");
                res = false;
            }
            else
            {
                argList.RemoveAt(posOfSwitch);
                un = argList[posOfSwitch];
                if (un.Length == 0 || un.StartsWith("/")){
                    Console.WriteLine("XX username arg present but needs a value");
                    return false;
                }
                foreach (string u in un.Split(","))
                {
                    var nuns = new string[uns.Length + 1];
                    uns.CopyTo(nuns,0);
                    nuns[nuns.Length - 1] = u.Trim();
                    uns = nuns;
                }
                argList.RemoveAt(posOfSwitch);
            }
            posOfSwitch = argList.IndexOf("/uf");
            if (posOfSwitch != -1)
            {
                argList.RemoveAt(posOfSwitch);
                unFld = argList[posOfSwitch];
                if (unFld.Length == 0 || unFld.StartsWith("/"))
                {
                    Console.WriteLine("XX username field arg present but needs a value");
                    return false;
                }
                argList.RemoveAt(posOfSwitch);
            }
            posOfSwitch = argList.IndexOf("/p");
            if (posOfSwitch == -1)
            {
                Console.WriteLine("XX need a password");
                res = false;
            }
            else
            {
                argList.RemoveAt(posOfSwitch);
                pw = argList[posOfSwitch];
                if(pw.Length == 0 || pw.StartsWith("/"))
                {
                    Console.WriteLine("XX password arg present but needs a value");
                    return false;
                }
                foreach (string p in pw.Split(","))
                {
                    var npws = new string[pws.Length + 1];
                    pws.CopyTo(npws, 0);
                    npws[npws.Length - 1] = p.Trim();
                    pws = npws;
                }
                argList.RemoveAt(posOfSwitch);
            }
            posOfSwitch = argList.IndexOf("/pf");
            if (posOfSwitch != -1)
            {
                argList.RemoveAt(posOfSwitch);
                pwFld = argList[posOfSwitch];
                if (pwFld.Length == 0 || pwFld.StartsWith("/"))
                {
                    Console.WriteLine("XX password field arg present but needs a value");
                    return false;
                }
                argList.RemoveAt(posOfSwitch);
            }
            posOfSwitch = argList.IndexOf("/lf");
            if (posOfSwitch != -1)
            {
                argList.RemoveAt(posOfSwitch);
                lgnBtn = argList[posOfSwitch];
                if (lgnBtn.Length == 0 || lgnBtn.StartsWith("/"))
                {
                    Console.WriteLine("XX login field arg present but needs a value");
                    return false;
                }
                argList.RemoveAt(posOfSwitch);
            }
            posOfSwitch = argList.IndexOf("/t");
            if (posOfSwitch != -1)
            {
                argList.RemoveAt(posOfSwitch);
                var success = int.TryParse(argList[posOfSwitch], out maxPara);
                if (maxPara == 0 || !success)
                {
                    Console.WriteLine("XX threads arg present but is 0 or cannot parse int");
                    return false;
                }
                argList.RemoveAt(posOfSwitch);
            }
            posOfSwitch = argList.IndexOf("/a");
            if (posOfSwitch != -1)
            {
                argList.RemoveAt(posOfSwitch);
                act = argList[posOfSwitch];
                foreach (string a in act.Split(","))
                {
                    var nacts = new int[acts.Length + 1];
                    var success = int.TryParse(a, out nacts[nacts.Length - 1]);
                    acts.CopyTo(nacts, 0);
                    acts = nacts;
                    if(!success || acts[acts.Length - 1] < 0)
                    {
                        Console.WriteLine("XX acts arg present but is less than 0 cannot be parsed");
                        return false;
                    }
                }
                argList.RemoveAt(posOfSwitch);
            }
            posOfSwitch = argList.IndexOf("/sid");
            if (posOfSwitch != -1)
            {
                argList.RemoveAt(posOfSwitch);
                sessIdName = argList[posOfSwitch];
                if (sessIdName.Length == 0 || sessIdName.StartsWith("/"))
                {
                    Console.WriteLine("XX session id arg present but needs a value");
                    return false;
                }
                argList.RemoveAt(posOfSwitch);
            }
            posOfSwitch = argList.IndexOf("/o");
            if (posOfSwitch != -1)
            {
                argList.RemoveAt(posOfSwitch);
                outLog = argList[posOfSwitch];
                if (outLog.Length == 0 || outLog.StartsWith("/"))
                {
                    Console.WriteLine("XX out log arg present but needs a value");
                    return false;
                }
                argList.RemoveAt(posOfSwitch);
            }
            posOfSwitch = argList.IndexOf("/s");
            if (posOfSwitch != -1)
            {
                argList.RemoveAt(posOfSwitch);
                silent = true;
            }
            posOfSwitch = argList.IndexOf("/ne");
            if (posOfSwitch != -1)
            {
                argList.RemoveAt(posOfSwitch);
                noExcep = true;
            }
            posOfSwitch = argList.IndexOf("/l");
            if (posOfSwitch != -1)
            {
                argList.RemoveAt(posOfSwitch);
                ol = File.CreateText(outLog);
                log = true;
            }
            if (acts.Length != uns.Length && acts.Length != pw.Length)
            {
                Console.WriteLine("XX uns, pws, and acts must all be the same length");
                return false;
            }
            else
            {
                for(var i = 0; i < acts.Length; i++)
                {
                    dfs.Add(new DeliciousFriend((ActsManager.acts)acts[i], uns[i], pws[i]));
                }
            }
            return res;
        }
    }
}