using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace task_parallel_exec
{
    public static class AsyncEx
    {
        public static async Task ParallelForEachAsync<T>(this IEnumerable<T> source, Func<T, Task> asyncAction, int maxDegreeOfParallelism = 10)
        {
            var semaphoreSlim = new SemaphoreSlim(maxDegreeOfParallelism);
            var tcs = new TaskCompletionSource<object>();
            var exceptions = new ConcurrentBag<Exception>();
            bool addingCompleted = false;

            foreach (T item in source)
            {
                await semaphoreSlim.WaitAsync();
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                asyncAction(item).ContinueWith(t =>
                 {
                     semaphoreSlim.Release();

                     if (t.Exception != null)
                     {
                         exceptions.Add(t.Exception);
                     }

                     if (Volatile.Read(ref addingCompleted) && semaphoreSlim.CurrentCount == maxDegreeOfParallelism)
                     {
                         tcs.SetResult(null);
                     }
                 });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            }

            Volatile.Write(ref addingCompleted, true);
            await tcs.Task;
            if (exceptions.Count > 0)
            {
                throw new AggregateException(exceptions);
            }
        }
    }
}