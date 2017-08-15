using System;
using System.Collections.Generic;
using System.Text;
using ToolKit.Models;
using System.Net;
using System.Threading.Tasks;
using System.Threading;

namespace ToolKit
{
    public interface IAdapter
    {
        Task<NodeInfo[]> GetNodesAsync(Hash accessKey, CancellationToken token = default(CancellationToken));

        Task<DirectoryInfo> ScanAsync(Hash accessKey, string path, CancellationToken token = default(CancellationToken));
        Task<OperationResult> CreateAsync(Hash accessKey, Hash nodeID, string name, string path, string extension, object meta, CancellationToken token = default(CancellationToken));
        Task<OperationResult> RenameAsync(Hash accessKey, Hash nodeID, Hash fileID, string newName, CancellationToken token = default(CancellationToken));

        Task UploadAsync(Hash accessKey, Hash nodeID, Hash fileID, System.IO.Stream dataStream, long size, CancellationToken token = default(CancellationToken));
        Task<byte[]> DownloadAsync(Hash accessKey, Hash nodeID, Hash fileID, CancellationToken token = default(CancellationToken));

        Task<Event[]> listenEventsAsync(Hash accessKey, DateTime timeStamp);
    }
}
