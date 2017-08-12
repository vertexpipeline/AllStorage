using System;
using System.Net;
using static System.Console;
using ToolKit;
using System.Threading;
using System.Threading.Tasks;

namespace Tester
{
    public class Program
    {
        static async Task Start()
        {
            foreach (IPAddress addr in await NetworkScaner.ScanAsync(
                IPAddress.Parse("192.168.1.0"),
                IPAddress.Parse("192.168.1.255"),
                600)) {
                WriteLine(addr);
            }
        }

        static void Main(string[] args)
        {
            WriteLine("start");
            var task = Start();
            Task.WaitAll(task);
            WriteLine("End");
            ReadKey();
        }
    }
}