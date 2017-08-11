using System;
using System.Net;
using static System.Console;
using ToolKit;

namespace Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            foreach (IPAddress addr in NetworkScaner.Scan(
                IPAddress.Parse("192.168.1.0"),
                IPAddress.Parse("192.168.1.255"),
                600)) {
                WriteLine(addr);
            }
            ReadKey();
        }
    }
}