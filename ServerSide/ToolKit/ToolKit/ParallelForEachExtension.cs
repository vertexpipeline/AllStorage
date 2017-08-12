using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using System.Linq;


public static class ForEachAsyncExtension
{
    public static Task ForEachAsync<T>(this IEnumerable<T> source, int dop, Func<T, Task> body)
    {
        return Task.WhenAll(from partition in Partitioner.Create(source).GetPartitions(dop)
                            select Task.Run(async delegate
                            {
                                using (partition)
                                    while (partition.MoveNext())
                                        await body(partition.Current).ConfigureAwait(false);
                            }));
    }
}