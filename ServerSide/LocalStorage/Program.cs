using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using static System.Console;
using ToolKit.Models.Packages;
using LocalStorage.Controllers;

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
                if(settings.key == null) {
                    WriteLine("Server provides public access");
                } else {
                    WriteLine($"Server key: {settings.key}");
                    WriteLine($"Key hash: {System.Net.WebUtility.UrlEncode(ToolKit.Hash.FromString(settings.key).ToString())}");
                }
                WriteLine("Initializing storaging repositories");
                APIController.packages = new List<Package>(JsonConvert.DeserializeObject<Package[]>(System.IO.File.ReadAllText("storageMeta.json")));
                WriteLine("Success");
            }catch(Exception ex) {
                WriteLine("Cannot load settings.\n Press any key...");
                return;
            }
            Console.WriteLine(DateTime.Now.ToString());
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>() 
                .UseUrls($"Http://192.168.1.95:{settings.port}")
                .Build();

                host.Run();
            
        }
    }
}
