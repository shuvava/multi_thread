using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace task_parallel_exec
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Application started");

            var messages = new List<string>();
            foreach (var i in Enumerable.Range(0, 100)) messages.Add($"hello world {i}");

            var s1 = Stopwatch.StartNew();
            TplUsage(messages);
            s1.Stop();
            var s2 = Stopwatch.StartNew();
            SemaphoreSlimUsage(messages).Wait();
            s2.Stop();
            Console.WriteLine("task submitted");
            const int m = 10000;
            Console.WriteLine(((double)(s1.Elapsed.TotalMilliseconds * 1000000) /
                               m).ToString("0.00 ns"));
            Console.WriteLine(((double)(s2.Elapsed.TotalMilliseconds * 1000000) /
                               m).ToString("0.00 ns"));

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }


        private static void TplUsage(IEnumerable<string> messages, int maxDegree = 10)
        {
            Parallel.ForEach(
                messages,
                new ParallelOptions {MaxDegreeOfParallelism = maxDegree},
                msg =>
                {
                    Process(msg).Wait();
                }
            );
        }

        private static Task SemaphoreSlimUsage(IEnumerable<string> messages, int maxDegree = 10)
        {
            return messages.ParallelForEachAsync(Process);
        }


        public static async Task Process(string msg)
        {
            await Task.Delay(100);
            Print(msg);
        }

        public static void Print(string msg)
        {
            Console.WriteLine($"Thread id {Thread.CurrentThread.ManagedThreadId} : {msg}");
        }
    }
}