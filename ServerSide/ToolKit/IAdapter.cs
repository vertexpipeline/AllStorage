using System;
using System.Collections.Generic;
using System.Text;
using ToolKit.Models;
using System.Net;
using System.Threading.Tasks;
using System.Threading;
using ToolKit.Models.Packages;
using ToolKit.Models.Nodes;
using ToolKit.Models.Event;

namespace ToolKit
{
    public interface IAdapter
    {
        Task<NodeInfo[]> GetNodesAsync(Hash accessKey, CancellationToken token = default(CancellationToken));
        Task<NodeInfo> GetInfoAsync(Hash accessKey, CancellationToken token = default(CancellationToken));

        Task<Package> ScanAsync(Hash accessKey, string path, CancellationToken token = default(CancellationToken));
        Task<OperationResult> AddFileAsync(Hash accessKey, Hash nodeID, string name, string path, string extension, CancellationToken token = default(CancellationToken));
        Task<OperationResult> MoveAsync(Hash accessKey, Hash nodeID, Hash fileID, string newPath, CancellationToken token = default(CancellationToken));
        Task<OperationResult> RenameAsync(Hash accessKey, Hash nodeID, Hash fileID, string newName, CancellationToken token = default(CancellationToken));
        Task<OperationResult> DeleteAsync(Hash accessKey, Hash nodeID, Hash fileID, CancellationToken token = default(CancellationToken));
        
        Task UploadAsync(Hash accessKey, Hash nodeID, Hash fileID, System.IO.Stream dataStream, CancellationToken token = default(CancellationToken));
        Task<byte[]> DownloadAsync(Hash accessKey, Hash nodeID, Hash fileID, CancellationToken token = default(CancellationToken));

        Task<Event[]> listenEventsAsync(Hash accessKey, DateTime timeStamp);
    }
}
