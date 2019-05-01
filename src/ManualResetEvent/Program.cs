using System;
using System.Threading;
using System.Threading.Tasks;


namespace ManualResetEvent
{
    internal class Program
    {
        private static readonly System.Threading.ManualResetEvent ManualResetEvent =
            new System.Threading.ManualResetEvent(true);


        private static void Main(string[] args)
        {
            Console.WriteLine("App started");
            Runner();
            Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] press Any key to pause");
            Console.ReadKey();
            ManualResetEvent.Reset();
            Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] press Any key to continue");
            Console.ReadKey();
            ManualResetEvent.Set();
            Console.ReadKey();
        }


        public static Task Runner()
        {
            return Task.Run(() =>
            {
                while (true)
                {
                    ManualResetEvent.WaitOne();
                    Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] {nameof(Runner)} has done operation {DateTime.Now:mm:ss.ms}");
                    Task.Delay(TimeSpan.FromMilliseconds(800));
                }
            });
        }
    }
}