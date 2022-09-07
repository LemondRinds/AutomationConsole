using System;
using System.Collections.Generic;
using System.IO;

namespace Automation
{
    public class Program
    {
        static int Main(string[] args)
        {
            int toDo = DoStuff.Stuff(args);
            return toDo;
        }

        // could add di and semaphores and threading and async to all this but not specified in reqs
        private static class DoStuff
        {
            public static int Stuff(string[] args)
            {
                try
                {
                    if (args.Length == 0)
                    {
                        Console.WriteLine(AppConfig.help);
                        Console.WriteLine(">> press enter to exit");
                        Console.ReadLine();
                        return 0;
                    }
                    var valid = AppConfig.ParseArgs(args);
                    if (valid == false)
                    {
                        Console.WriteLine(">> press enter to exit");
                        Console.ReadLine();
                        return 0;
                    }



                    if (AppConfig.DEBUG)
                        AppConfig.WriteOut(">> press enter to exit", true);
                    return 1;
                }
                catch (Exception ex)
                {
                    if (AppConfig.DEBUG)
                    {
                        AppConfig.ErrHand(ex);
                        AppConfig.WriteOut(">> press enter to exit", true);
                    }
                    return 0;
                }
                finally
                {
                }
            }
        }
        /// <summary>
        /// Helper functions and methods, config code, static strings, global settings, error handler etc
        /// </summary>
        private static class AppConfig
        {
            // --in and --out vars, exception and debug vars, help text, static strings
            public static string inFl, otFl;
            public static bool DEBUG = true;
            public static readonly string help = $"winner.exe aka DRVC <<{EOL}" +
                $">> process an input file of cards dealt to find a winner {EOL}{EOL}" +
                $"OUTPUTS <<{EOL}" +
                $">> single file of either{EOL}" +
                $">> Name:Score{EOL}{EOL}" +
                $">> or a cdl of the above{EOL}{EOL}" +
                $"INPUTS <<{EOL}" +
                $">> {"switch",-20}             {"required",-10}    {"info",-10}{EOL}" +
                $">> {"--in inFile.txt",-20}    {"X",-10}           {"relative or absolute file path to txt file for processing"}{EOL}" +
                $">> {"--out outFile.txt",-20}  {"X",-10}           {"relative or absolute file path in which to output results"}{EOL}" +
                $"ERRORS <<{EOL}" +
                $">> Issues with input file will return an output file of {ERROR}, otherwise expect more via this window{EOL}{EOL}"
                , EOL = Environment.NewLine, ERROR = "ERROR";
            /// <summary>
            /// Error Handler, logs to console and tries to include stack trace and message from exception if there was one
            /// </summary>
            /// <param name="ex">Exception</param>
            /// <param name="msg">string to output to console</param>
            public static void ErrHand(Exception ex, string msg = null)
            {
                if (!DEBUG || ex != null)
                {
                    string otpt = msg ?? "XX an unhandled exception occurred";
                    if (ex != null)
                        otpt += EOL + "XX " + (ex.Message.Length > 0 ? ex.Message : "") + (ex.StackTrace.Length > 0 && string.IsNullOrEmpty(msg) ? EOL + ex.StackTrace : "");
                    Console.WriteLine(otpt);
                }
            }
            /// <summary>
            /// Only usefyl if DEBUG is true when compiled
            /// Tries to write a message out to the console and can wait for enter key
            /// </summary>
            /// <param name="msg">string to output to console</param>
            /// <param name="rdIn">bool = false, wait for enter key</param>
            public static void WriteOut(string msg, bool rdIn = false)
            {
                if (!DEBUG)
                {
                    Console.WriteLine(msg);
                    if (rdIn)
                        Console.ReadLine();
                }
            }
            /// <summary>
            /// Handles parsing args to vars and outputting info if startup params aren't set
            /// </summary>
            /// <param name="args">string[] of items passed to the app</param>
            /// <returns>bool, true means args were parsed</returns>
            public static bool ParseArgs(string[] args)
            {
                // args are --in inFile.txt and --out outFile.txt
                // no specs on expected file types or validating data before processing
                var argList = new List<string>(args);
                bool res = true;
                int posOfSwitch = argList.IndexOf("--in");
                if (posOfSwitch == -1)
                {
                    Console.WriteLine("XX need an --in inFile.txt");
                    res = false;
                }
                else
                {
                    argList.RemoveAt(posOfSwitch);
                    inFl = argList[posOfSwitch];
                    if (inFl.Length == 0 || inFl.StartsWith("--"))
                    {
                        Console.WriteLine("XX --in arg present but needs a value");
                        return false;
                    }
                    argList.RemoveAt(posOfSwitch);
                }
                posOfSwitch = argList.IndexOf("--out");
                if (posOfSwitch == -1)
                {
                    Console.WriteLine("XX need an --out outFile.txt");
                    res = false;
                }
                else
                {
                    argList.RemoveAt(posOfSwitch);
                    inFl = argList[posOfSwitch];
                    if (inFl.Length == 0 || inFl.StartsWith("--"))
                    {
                        Console.WriteLine("XX --out arg present but needs a value");
                        return false;
                    }
                    argList.RemoveAt(posOfSwitch);
                }
                return res;
            }
        }
    }
}
