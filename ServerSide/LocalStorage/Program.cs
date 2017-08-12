using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using static System.Console;

namespace LocalStorage
{
    public class Program
    {
        public static Settings settings;

        public static void Main(string[] args)
        {
            try {
                settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText("settings.json"));
                WriteLine("Starting.");
                WriteLine($"Server key: {settings.key}");
                WriteLine($"Key hash: {System.Net.WebUtility.UrlEncode(ToolKit.Hash.FromString(settings.key).ToString())}");
            }catch(Exception ex) {
                WriteLine("Cannot load settings.\n Press any key...");
                return;
            }

            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .UseUrls($"Http://localhost:{settings.port}")
                .Build();

                host.Run();
        }
    }
}
