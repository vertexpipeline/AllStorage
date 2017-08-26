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
            // files initialization is in the Program class
            passH = (Program.settings.key != null) ? Hash.FromString(Program.settings.key) : null;
        }

        private bool ValidateKey()
        {
            string key;
            try {
                if (passH != null) {
                    key = Request.Query["accessKey"][0] as string;
                    var hash = new Hash(key);
                    if (hash.Equals(passH)) {
                        return true;
                    } else {
                        return false;
                    }
                } else {
                    return true;
                }
               
            } catch (Exception ex) {
                return false;
            }
        }

        static Hash passH;

        [Route("info")]
        public async Task<OperationResult> Info()
        {
            string key;
            try { key = Request.Query["accessKey"][0]; } catch (Exception ex) { key = null; }
            if(key != null) {
                if(!ValidateKey())
                    return new OperationResult { state = OperationState.invalid_key };
            }

            var ip = await NetworkScaner.GetCurrentIPAsync();
            return new OperationResult
            {
                result = new NodeInfo
                {
                    address = ip.ToString() + $":{Program.settings.port}",
                    ID = Hash.FromString(Dns.GetHostName()),
                    name = Dns.GetHostName(),
                    needKey = passH != null,
                    accessKey = passH,
                    nodes = new NodeInfo[0]
                }
            };
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

        public static List<Package> packages = new List<Package>();

        public static Task SaveFilesInfoAsync()
        {
            return Task.Run(() => {
                try {
                    var json = JsonConvert.SerializeObject(packages, Formatting.Indented);
                    System.IO.File.WriteAllText("storageMeta.json", json);
                }catch(Exception ex) {

                }
            });
        }

        string getParString(string name)
        {
            try {
                return Request.Query[name][0] as string;
            }catch(Exception ex) {
                return null;
            }
        }

        [HttpPost("addPackage")]
        public async Task<OperationResult> AddPackage([FromBody]Package package)
        {
            if (ValidateKey()) {
                try {
                    var hash = Hash.FromString(package.packageName);
                    //if (packages.Any(p => p.packageID.Equals(hash))) {
                    //    return new OperationResult { state = OperationState.package_already_exist };
                    //}

                    if (package.path == null) package.path = "";

                    void setID(FolderInfo f, string currentPath)
                    {
                        f.packageID = hash;
                        string fPath = currentPath;
                        if(!(f is Package)) {
                            fPath = currentPath + (currentPath != "" ? $"/{f.name}" : f.name);
                            f.path = currentPath;
                        } 

                        if (f.folders != null)
                            foreach (var childF in f.folders) {
                                setID(childF, fPath);
                                childF.path = fPath;
                            }

                        if (f.files != null)
                            foreach (var file in f.files) {
                                file.packageID = hash;
                                file.path = fPath;
                                file.fileID = Hash.FromString(file.name);
                            }
                    }

                    setID(package, package.path);
                    package.name = package.packageName;
                    package.packageID = hash;
                    //package.path = path;
                    packages.Add(package);
                    SaveFilesInfoAsync();
                    return new OperationResult { state = OperationState.success };
                } catch (Exception ex) {
                    return new OperationResult { state = OperationState.invalid_arguments_list };
                }
            } else {
                return new OperationResult { state = OperationState.invalid_key };
            }
        }

        public static FolderInfo SearchFolder(Package pkg, string targetPath)
        {
            FolderInfo scanFolder(FolderInfo folder, string path)
            {
                if (folder.path == path) {
                    return folder;
                } else if (path.StartsWith(folder.path)) {
                    foreach (var child in folder.folders) {
                        var found = scanFolder(child, path);
                        if (found != null)
                            return found;
                    }
                }
                return null;
            }
           return scanFolder(pkg, targetPath);
        }

        [Route("scan")]
        public async Task<OperationResult> Scan()
        {
            if (ValidateKey()) {
                var dirs = new List<FolderInfo>();
                var files = new List<FileInfo>();
                var res = new ScanResult();
                

                var path = getParString("path");
                res.path = path;
                var packagesList = getParString("packageIDs")?.Split(',');

                if (path == null)
                    path = "";

                if (packagesList != null) {

                } else {
                    var searchList = new List<Package>();
                    foreach (var pkg in packages) {
                         if (pkg.path.StartsWith(path)) {
                            searchList.Add(pkg);
                        }
                    }
                    foreach (var pkg in searchList) {
                        var folder = SearchFolder(pkg, path);
                        if (folder != null) {
                            res.folder.AddRange(folder.folders ?? new List<FolderInfo>());
                            res.files.AddRange(folder.files ?? new List<FileInfo>());
                        } 
                    }
                }

                 return new OperationResult { state = OperationState.success, result = res };
            } else {
                return new OperationResult { state = OperationState.invalid_key };
            }

        }
        //[Route("createFile")]
        //public async Task<OperationResult> CreateFile()
        //{
        //    if(ValidateKey()) {
        //        var name = Request.Query["name"][0] as string;
        //        var path = Request.Query["path"][0] as string;
        //        var ext = Request.Query["extension"][0] as string;
        //        var size = Convert.ToInt64(Request.Query["size"][0]);

        //        var info = new FileInfo
        //        {
        //            fileID = Hash.FromString($"{path}/{name}.{ext}"),
        //            name = name,
        //            extension = ext,
        //            size = size,
        //            path = path,
        //            state = FileState.created
        //        };

        //        var node = SearchFolder(path);

        //        if(node != null) {
        //            node.files.Add(info);
        //        } else {
        //            return new OperationResult{
        //                state = OperationState.folder_not_found,
        //                result = new Nothing()
        //            };
        //        }

        //        return new OperationResult {
        //            result = info,
        //            state = OperationState.success
        //        };

        //    } else {
        //        return new OperationResult { state = OperationState.invalid_key, result = new Nothing() };
        //    }
        //}

        //[Route("createFolder")]
        //public async Task<OperationResult> CreateFolder()
        //{
        //    if (ValidateKey()) {
        //        var name = Request.Query["name"][0] as string;
        //        var path = Request.Query["path"][0] as string;

        //        var info = new FolderInfo
        //        {
        //            name = name,
        //            path = path,
        //            files = new List<FileInfo>(),
        //            folders = new List<FolderInfo>()
        //        };

        //        var node = SearchFolder(path);

        //        if (node != null) {
        //            node.folders.Add(info);
        //        } else {
        //            return new OperationResult
        //            {
        //                state = OperationState.folder_not_found,
        //                result = new Nothing()
        //            };
        //        }

        //        return new OperationResult
        //        {
        //            result = info,
        //            state = OperationState.success
        //        };

        //    } else {
        //        return new OperationResult { state = OperationState.invalid_key, result = new Nothing() };
        //    }
        //}

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


    }
}
