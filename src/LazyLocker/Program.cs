using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;


namespace LazyLocker
{
    internal class Program
    {
        private static bool _cached;
        private static readonly SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1);


        private static void Main(string[] args)
        {
            Console.WriteLine("Application started");
            var tasks = new List<Task>();

            tasks.Add(LongRunAsync());
            tasks.Add(LongRunAsync());

            Task.WaitAll(tasks.ToArray());
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }


        private static async Task<string> LongRunAsync()
        {
            var result = "Result";

            if (_cached)
            {
                Console.WriteLine("Use cache");
            }
            else
            {
                await semaphoreSlim.WaitAsync();

                if (_cached) // double check on lock
                {
                    Console.WriteLine("Use cache");

                    return result;
                }

                await Task.Delay(TimeSpan.FromSeconds(2));
                _cached = true;
                semaphoreSlim.Release();
                Console.WriteLine("Do long work");
            }

            Console.WriteLine("Long run task completed");

            return result;
        }
    }
}
