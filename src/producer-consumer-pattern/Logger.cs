using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace producer_consumer_pattern
{
    public class Logger : IDisposable
    {
        private readonly DatabaseFacade _facade;
        private readonly BlockingCollection<string> _queue =
            new BlockingCollection<string>();
 
        private readonly Task _saveMessageTask;
 
        public Logger(DatabaseFacade facade) =>
            (_facade, _saveMessageTask) = (facade, Task.Run(SaveMessage));
 
        public void Dispose() => _queue.CompleteAdding();
 
        public void WriteLine(string message) => _queue.Add(message);
 
        private async Task SaveMessage()
        {
            foreach (var message in _queue.GetConsumingEnumerable())
            {
                // "Saving" message to the file
                Console.WriteLine($"Logger: {message}");
 
                // And to our database through the facade
                await _facade.SaveAsync(message);
            }
        }
    }
}