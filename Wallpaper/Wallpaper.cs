using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Collections.Generic;
using CommandLine;
using Newtonsoft.Json;

namespace Wallpaper
{
    public class Monitor
    {
        [JsonProperty("id")]
        public long? Id;

        [JsonProperty("path")]
        public string Path;
    }

    public class Wallpaper
    {
        [Verb("set", HelpText = "Set wallpaper for screen.")]
        class SetOptions 
        {
            [Option('i', "id", Required = true, HelpText = "Screen Id.")]
            public long? Id { get; set; }

            [Option('p', "path", Required = true, HelpText = "Wallpaper location.")]
            public string Wallpaper { get; set; }
        }

        [Verb("get", HelpText = "Get screen wallpaper.")]
        class GetOptions 
        {

        }

        static int Main(string[] args) 
        {
            return CommandLine.Parser.Default.ParseArguments<SetOptions, GetOptions>(args)
                .MapResult(
                    (SetOptions opts) => RunSetAndReturnExitCode(opts),
                    (GetOptions opts) => RunGetAndReturnExitCode(opts),
                    errs => HandleParseError(errs));
        }

        static int RunSetAndReturnExitCode(SetOptions opts)
        {
            if (!System.IO.File.Exists(opts.Wallpaper)) {
                Console.Out.WriteLine(JsonConvert.SerializeObject(new Dictionary<string, string>(){{"error", $"Wallpaper '{opts.Wallpaper}' is not exists"}}));
                return 1;
            }

            SetDesktopWallpaper(opts.Wallpaper);

            Monitor monitor = new Monitor();
            monitor.Id = opts.Id;
            monitor.Path = opts.Wallpaper;

            Console.Out.WriteLine(JsonConvert.SerializeObject(monitor));
            return 0;
        }

        static int RunGetAndReturnExitCode(GetOptions opts)
        {
            Monitor monitor = new Monitor();
            monitor.Id = 0;
            monitor.Path = GetDesktopWallpaper();

            Console.Out.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(monitor));
            return 0;
        }

        static int HandleParseError(IEnumerable<Error> errs) 
        {
            return 1;
        }

        const int SPI_SETDESKWALLPAPER = 0x0014;
        const int SPI_GETDESKWALLPAPER = 0x0073;
        const int SPIF_UPDATEINIFILE = 0x01;
        const int SPIF_SENDWININICHANGE = 0x02;
        const int MAX_PATH = 260;

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int winIni);

        public static string GetDesktopWallpaper()
        {
            string wallpaper = new string('\0', MAX_PATH);
            SystemParametersInfo(SPI_GETDESKWALLPAPER, (int)wallpaper.Length, wallpaper, 0);
            return wallpaper.Substring(0, wallpaper.IndexOf('\0'));
        }

        public static int SetDesktopWallpaper(string file)
        {
            return SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, file, SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
        }
    }
}