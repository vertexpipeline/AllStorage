using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using System.Linq;

namespace ToolKit
{
    public class NetworkScaner
    {
        public static async Task<IPAddress[]> ScanAsync(IPAddress start, IPAddress end, short serverPort)
        {
            var startBytes = start.GetAddressBytes();
            var endBytes = end.GetAddressBytes();
            var foundIPs = new List<IPAddress>();
            var addresses = new List<IPAddress>();

            for (int i = startBytes[0]; i < endBytes[0] + 1; i++)
                for (int j = startBytes[1]; j < endBytes[1] + 1; j++)
                    for (int k = startBytes[2]; k < endBytes[2] + 1; k++)
                        for (int l = startBytes[3]; l < endBytes[3] + 1; l++) {
                            var ip = new IPAddress(new Byte[] {
                                    (byte)i,
                                    (byte)j,
                                    (byte)k,
                                    (byte)l
                                });
                            addresses.Add(ip);
                        }

            var client = new HttpClient();
            client.Timeout = new TimeSpan(0, 0, 1);
            await addresses.ForEachAsync(40, async (ip) => {
                try {
                    var ipString = "http://" + ip.ToString() + ":" + serverPort.ToString();
                    var res = await client.GetAsync(ipString);
                    if (res.StatusCode == HttpStatusCode.OK)
                        foundIPs.Add(ip);

                } catch (Exception ex) {

                }
            });
            return foundIPs.ToArray();
        }
        public static async Task<IPAddress> GetCurrentIPAsync()
        {
            return (await Dns.GetHostAddressesAsync(Dns.GetHostName())).First(p => p.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
        }
    }
}
