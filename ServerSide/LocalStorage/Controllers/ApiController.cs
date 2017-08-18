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
using ToolKit.Models.Event;
using ToolKit.Models.Nodes;
using ToolKit.Models.Packages;

namespace LocalStorage.Controllers
{
    [Route("api")]
    public class APIController : Controller
    {
        static System.Threading.Timer savingTimer;

        static APIController()
        {
            rootDir.folders = new List<FolderInfo>(JsonConvert.DeserializeObject<FolderInfo[]>(System.IO.File.ReadAllText("storageMeta.json")));
        }

        private bool ValidateKey()
        {
            string key;
            Hash passH = Hash.FromString(Program.settings.key);
            try {
                key = Request.Query["accessKey"][0] as string;
                var hash = new Hash(key);
                if (hash == passH) {
                    return true;
                }
                return false;
            } catch (Exception ex) {
                return false;
            }
        }

        Hash passH = Hash.FromString(Program.settings.key);

        [Route("info")]
        public async Task<OperationResult> Info()
        {
            if (ValidateKey()) {
                var ip = await NetworkScaner.GetCurrentIPAsync();
                return new OperationResult
                {
                    result = new NodeInfo
                    {
                        accessKey = passH,
                        address = ip.ToString() + $":{Program.settings.port}",
                        ID = Hash.FromString(Dns.GetHostName()),
                        name = Dns.GetHostName(),
                        nodes = new NodeInfo[0]
                    }
                };
            } else { 
                return new OperationResult { state = OperationState.invalid_key, result = new Nothing() };
            }
        }

        [Route("nodes")]
        public async Task<OperationResult> Nodes()
        {
            if (ValidateKey()) {
                var ip = await NetworkScaner.GetCurrentIPAsync();
                return new OperationResult
                {
                    result = new NodeInfo[] { }
                };
            } else {

                return new OperationResult { state = OperationState.invalid_key, result = new Nothing() };
            }
        }

        [Route("ping")]
        public string Ping()
        {
            return "pong";
        }

        public static FolderInfo rootDir = new FolderInfo();

        public static FolderInfo SearchFolder(string path)
        {
            var curNode = rootDir;
            foreach (var folder in path.Split('/')) {
                bool found = false;
                foreach (var child in curNode.folders) {
                    if (child.name == folder) {
                        found = true;
                        curNode = child;
                        break;
                    }
                }
                if(!found)
                    return null;
            }
            return curNode;
        }

        public static Task SaveFilesInfoAsync()
        {
            return Task.Run(() => {
                var json = JsonConvert.SerializeObject(rootDir.folders, Formatting.Indented);
                System.IO.File.WriteAllText("storageMeta.json", json);
            });
        }

        [Route("createFile")]
        public async Task<OperationResult> CreateFile()
        {
            if(ValidateKey()) {
                var name = Request.Query["name"][0] as string;
                var path = Request.Query["path"][0] as string;
                var ext = Request.Query["extension"][0] as string;
                var size = Convert.ToInt64(Request.Query["size"][0]);

                var info = new FileInfo
                {
                    fileID = Hash.FromString($"{path}/{name}.{ext}"),
                    name = name,
                    extension = ext,
                    size = size,
                    path = path,
                    state = FileState.created
                };

                var node = SearchFolder(path);

                if(node != null) {
                    node.files.Add(info);
                } else {
                    return new OperationResult{
                        state = OperationState.folder_not_found,
                        result = new Nothing()
                    };
                }


                return new OperationResult {
                    result = info,
                    state = OperationState.success
                };
                
            } else {
                return new OperationResult { state = OperationState.invalid_key, result = new Nothing() };
            }
        }

        [Route("createFolder")]
        public async Task<OperationResult> CreateFolder()
        {
            if (ValidateKey()) {
                var name = Request.Query["name"][0] as string;
                var path = Request.Query["path"][0] as string;
                
                var info = new FolderInfo
                {
                    name = name,
                    path = path,
                    files = new List<FileInfo>(),
                    folders = new List<FolderInfo>()
                };

                var node = SearchFolder(path);

                if (node != null) {
                    node.folders.Add(info);
                } else {
                    return new OperationResult
                    {
                        state = OperationState.folder_not_found,
                        result = new Nothing()
                    };
                }

                return new OperationResult
                {
                    result = info,
                    state = OperationState.success
                };

            } else {
                return new OperationResult { state = OperationState.invalid_key, result = new Nothing() };
            }
        }

        //[Route("upload")]
        //public async void Upload()
        //{
        //    if (ValidateKey()) {
        //        var id = Hash.FromBase64(Request.Query["fileID"][0]);
        //        var size = Convert.ToInt64(Request.Query["size"][0]);

        //        var file = files.FirstOrDefault(p => p.fileID == id);
        //        if(file != null) {
        //            if(file.state == FileState.created) {
        //                using(var stream = System.IO.File.Create($"files/{WebUtility.UrlEncode(file.fileID.ToString())}.file")) {
        //                    await Request.Body.CopyToAsync(stream, 1024);
        //                    var hash = await Hash.FromStreamAsync(stream);
        //                    file.dataHash = hash;
        //                    file.size = size;
        //                    file.state = FileState.uploaded;
        //                    SaveFilesInfoAsync();
        //                }

        //            } else {
        //                Response.StatusCode = (int)HttpStatusCode.NotModified;
        //            }
        //        } else {
        //            Response.StatusCode = (int)HttpStatusCode.NotFound;
        //        }
        //    } else {
        //        Response.StatusCode = (int)HttpStatusCode.Unauthorized;
        //    }
        //}

        [Route("scan")]
        public async Task<OperationResult> Scan()
        {
            var dirs = new List<FolderInfo>();
            var files = new List<FileInfo>();
            if (ValidateKey()) {
                var path = Request.Query["path"][0] as string;
                var folder = SearchFolder(path);

                if (folder != null) {
                    return new OperationResult { state = OperationState.success, result = new ScanResult{ files = folder.files, folders = folder.folders } };
                }
                return new OperationResult { state = OperationState.folder_not_found, result = new Nothing() };

            } else {
                return new OperationResult { state = OperationState.invalid_key, result = new Nothing() };
            }
        }
    }
}
