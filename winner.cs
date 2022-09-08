using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace App
{
    public class Winner
    {
        static int Main(string[] args)
        {
            // parse args and check in file exists etc first
            try
            {
                // quick check to display help text
                if (args.Length == 0)
                {
                    Console.WriteLine(AppConfig.help);
                    Console.WriteLine(AppConfig.entrMsg);
                    Console.ReadLine();
                    return 0;
                }
                // try getting args
                var valid = AppConfig.ParseArgs(args);
                if (valid == false)
                {
                    Console.WriteLine(AppConfig.entrMsg);
                    Console.ReadLine();
                    return 0;
                }
                // make sure input file exists
                if (!File.Exists(AppConfig.inFl))
                {
                    Console.WriteLine("XX --in file does not exist");
                    Console.WriteLine(AppConfig.entrMsg);
                    Console.ReadLine();
                    return 0;
                }
            }
            catch (Exception ex)
            {
                if (AppConfig.DEBUG)
                {
                    AppConfig.ErrHand(ex);
                    Console.WriteLine(AppConfig.entrMsg);
                    Console.ReadLine();
                }
                return 0;
            }

            // try catch for the score tallying (finally used for getting string from builder)
            StringBuilder toWrt = new StringBuilder();
            try
            {
                // find winner
                toWrt = Dealer.FindWinner();
            }
            catch (Exception ex)
            {
                toWrt = new StringBuilder(AppConfig.ERROR);
                if (AppConfig.DEBUG)
                {
                    AppConfig.ErrHand(ex);
                    Console.WriteLine(AppConfig.entrMsg);
                    Console.ReadLine();
                    return 0;
                }
            }
            finally
            {
                // create output file and write
                var flInfo = new FileInfo(AppConfig.otFl);
                flInfo.Directory.Create();
                File.WriteAllText(flInfo.FullName, toWrt.ToString());
            }
            return 1;
        }
        /// <summary>
        /// Dealer deals with the cards
        /// Left this object in here from a restructure pass
        /// Could be used to setup some DI or broken down a bit for unit testing
        /// </summary>
        private static class Dealer
        {
            /// <summary>
            /// Finds the winner from a set of hands read in from an input file
            /// </summary>
            /// <returns>StringBuilder with name[,name...]:score of the winner(s)</returns>
            public static StringBuilder FindWinner()
            {
                // string builder is better than string i hear
                StringBuilder otFlBldr = new StringBuilder();
                int highest = 0;
                // read in lines and keep highest tally after simple validation
                string[] lines = File.ReadLines(AppConfig.inFl).ToArray();
                if (lines.Length > 5)
                    throw new InvalidOperationException("input file not 5 lines");
                for (int i = 0; i < lines.Length; i++)
                {
                    var lnItms = lines[i].Split(":");
                    if (lnItms.Length != 2)
                        throw new InvalidOperationException("not 2 items in line " + i + 1);
                    var scores = lnItms[1].Split(",");
                    if (scores.Length != 5)
                        throw new InvalidOperationException("not 5 scores on line " + i + 1);
                    int tally = 0;
                    // convert suit and values using switches and an int tryparse
                    for (int si = 0; si < scores.Length; si++)
                    {
                        var scr = scores[si];
                        var suit = scr[scr.Length - 1];
                        tally += suit switch
                        {
                            'C' => 1,
                            'D' => 2,
                            'H' => 3,
                            'S' => 4,
                            _ => throw new InvalidOperationException("bad suit line " + i + 1)
                        };
                        var card = scr.Substring(0, scr.Length - 1);
                        if (int.TryParse(card, out int cardVal))
                        {
                            if (cardVal > 10 || cardVal < 2)
                                throw new InvalidOperationException("out of bounds card int line " + i + 1);
                            tally += cardVal;
                        }
                        else
                        {
                            tally += card switch
                            {
                                "A" => 1,
                                "J" => 11,
                                "Q" => 12,
                                "K" => 13,
                                _ => throw new InvalidOperationException("bad face card line " + i + 1)
                            };
                        }
                    }
                    // this part is self explanatory
                    if (tally > highest)
                    {
                        highest = tally;
                        otFlBldr = new StringBuilder(lnItms[0]);
                    }
                    else if (tally == highest)
                    {
                        otFlBldr.Append("," + lnItms[0]);
                    }
                }
                otFlBldr.Append(":" + highest);
                return otFlBldr;
            }
        }
        /// <summary>
        /// Config code, parse args, static strings, global settings, error handler etc
        /// </summary>
        private static class AppConfig
        {
            // --in and --out vars, exception and debug vars, help text, static strings
            public static string inFl, otFl;
            public static bool DEBUG = false;
            public static readonly string EOL = Environment.NewLine, ERROR = "ERROR",
                entrMsg = ">> press enter to exit",
                help = $"winner.exe aka DRVC <<{EOL}" +
                    $">> process an input file of cards dealt to find a winner {EOL}" +
                    $"{EOL}OUTPUTS <<{EOL}" +
                    $">> single file of format{EOL}" +
                    $">> name[,name...]:score{EOL}" +
                    $"{EOL}INPUTS <<{EOL}" +
                    $">> {"switch",-20}{"required",-10}{"info",-50}{EOL}" +
                    $">> {"--in inFile.txt",-20}{"X",-10}{"relative or absolute file path to txt file for processing",-50}{EOL}" +
                    $">> {"--out outFile.txt",-20}{"X",-10}{"relative or absolute file path in which to output results",-50}{EOL}" +
                    $">> {"--dbg",-20}{"",-10}{"debug flag",-50}{EOL}" +
                    $"{EOL}ERRORS <<{EOL}" +
                    $">> issues with input file will return an output file of {ERROR}, otherwise expect more via this window{EOL}";
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
            /// Handles parsing args to vars and outputting info if startup params aren't set
            /// </summary>
            /// <param name="args">string[] of items passed to the app</param>
            /// <returns>bool, true means args were parsed</returns>
            public static bool ParseArgs(string[] args)
            {
                if(args.Length < 2){
                    Console.WriteLine("XX too few arguments to begin parsing");
                    return false;
                }
                var argList = new List<string>(args);
                bool res = true;
                // repeat this if/else-remove-from-args pattern
                // --in
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
                // --out
                posOfSwitch = argList.IndexOf("--out");
                if (posOfSwitch == -1)
                {
                    Console.WriteLine("XX need an --out outFile.txt");
                    res = false;
                }
                else
                {
                    argList.RemoveAt(posOfSwitch);
                    otFl = argList[posOfSwitch];
                    if (otFl.Length == 0 || otFl.StartsWith("--"))
                    {
                        Console.WriteLine("XX --out arg present but needs a value");
                        return false;
                    }
                    argList.RemoveAt(posOfSwitch);
                }
                // --dbg
                posOfSwitch = argList.IndexOf("--dbg");
                if (posOfSwitch > -1)
                {
                    Console.WriteLine("++ debug set");
                    DEBUG = true;
                }
                return res;
            }
        }
    }
}