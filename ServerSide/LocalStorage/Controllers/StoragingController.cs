#pragma warning disable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ToolKit;
using ToolKit.Models;
using System.Collections.Concurrent;
using Newtonsoft.Json;
using System.Net;

namespace LocalStorage.Controllers
{
    [Route("[controller]")]
    public class StoragingController : Controller
    {
        static StoragingController()
        {
            foreach (FileInfo f in JsonConvert.DeserializeObject<FileInfo[]>(System.IO.File.ReadAllText("files.json"))) { 
                files.Add(f);
            }
        }

        private bool ValidateKey()
        {
            string key;
            Hash passH = Hash.FromString(Program.settings.key);
            try { key = Request.Query["accessKey"][0] as string; } catch (Exception ex) { return false; }
            var hash = new Hash(key);
            if (hash == passH) {
                return true;
            }
            return false;
        }

        public static ConcurrentBag<FileInfo> files = new ConcurrentBag<FileInfo>();

        public Task SaveFilesInfoAsync()
        {
            return Task.Run(() => {
                var json = JsonConvert.SerializeObject(files, Formatting.Indented);
                System.IO.File.WriteAllText("files.json", json);
            });
        }

        [Route("create")]
        public async Task<OperationResult> Create()
        {
            if(ValidateKey()) {
                var name = Request.Query["name"][0] as string;
                var path = Request.Query["path"][0] as string;
                var ext = Request.Query["extension"][0] as string;
        
                var info = new FileInfo
                {
                    fileID = Hash.FromString($"{path}\\{name}.{ext}"),
                    name = name,
                    extension = ext,
                    size = 0,
                    path = path,
                    state = FileState.created
                };

                files.Add(info);

                SaveFilesInfoAsync();

                return new OperationResult {
                    result = info,
                    state = OperationState.success
                };
                
            } else {
                Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return null;
            }
        }

        [Route("upload")]
        public async void Upload()
        {
            if (ValidateKey()) {
                var id = Hash.FromBase64(Request.Query["fileID"][0]);
                var size = Convert.ToInt64(Request.Query["size"][0]);

                var file = files.FirstOrDefault(p => p.fileID == id);
                if(file != null) {
                    if(file.state == FileState.created) {
                        using(var stream = System.IO.File.Create($"files/{WebUtility.UrlEncode(file.fileID.ToString())}.file")) {
                            await Request.Body.CopyToAsync(stream, 1024);
                            var hash = await Hash.FromStreamAsync(stream);
                            file.dataHash = hash;
                            file.state = FileState.uploaded;
                            SaveFilesInfoAsync();
                        }
                        
                    } else {
                        Response.StatusCode = (int)HttpStatusCode.NotModified;
                    }
                } else {
                    Response.StatusCode = (int)HttpStatusCode.NotFound;
                }
            } else {
                Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            }
        }
    }
}
