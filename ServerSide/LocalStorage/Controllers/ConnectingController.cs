using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ToolKit;
using ToolKit.Models;
using System.Net;
using System.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LocalStorage.Controllers
{
    [Route("[controller]")]
    public class ConnectingController : Controller
    {
        private Hash passH = Hash.FromString(Program.settings.key);

        private bool ValidateKey()
        { 
            string key;
            try { key = Request.Query["accessKey"][0] as string; } catch (Exception ex) { return false; }
            var hash = new Hash(key);
            if (hash == passH) {
                return true;
            }
            return false;
        }

        [Route("nodes")]
        public async Task<NodeInfo[]> Nodes()
        {
            if(ValidateKey()) {
                var ip = await NetworkScaner.GetCurrentIPAsync();
                return new NodeInfo[] { new NodeInfo {
                    accessKey = passH,
                    address = ip.ToString() + $":{Program.settings.port}",
                    ID = Hash.FromString(Dns.GetHostName()),
                    name = Dns.GetHostName(),
                    nodes = new NodeInfo[0]
                } };
            } else {
                Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return new NodeInfo[0];
            }
        }
    }
}
