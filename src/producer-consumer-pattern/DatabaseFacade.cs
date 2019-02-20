using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace producer_consumer_pattern
{
    public class DatabaseFacade : IDisposable
    {
        private readonly BlockingCollection<(string item, TaskCompletionSource<string> result)> _queue =
            new BlockingCollection<(string item, TaskCompletionSource<string> result)>();
        private readonly Task _processItemsTask;
 
        public DatabaseFacade() => _processItemsTask = Task.Run(ProcessItems);
 
        public void Dispose() => _queue.CompleteAdding();
 
        public Task SaveAsync(string command)
        {
            // to fix issue uncomment line
            //var tcs = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
            var tcs = new TaskCompletionSource<string>();
            _queue.Add((item: command, result: tcs));
            return tcs.Task;
        }
 
        private async Task ProcessItems()
        {
            foreach (var item in _queue.GetConsumingEnumerable())
            {
                Console.WriteLine($"DatabaseFacade: executing '{item.item}'...");
 
                // Waiting a bit to emulate some IO-bound operation
                await Task.Delay(100);
                item.result.SetResult("OK");
                Console.WriteLine("DatabaseFacade: done.");
            }
        }
    }
}