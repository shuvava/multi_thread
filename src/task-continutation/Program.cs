using System;
using System.Threading;
using System.Threading.Tasks;

namespace task_continutation
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Application started!");
            await WithAsync(TaskCreationOptions.None);
            await WithAsync(TaskCreationOptions.RunContinuationsAsynchronously);
            Console.ReadKey();
        }

        static async Task WithAsync(TaskCreationOptions options)
        {
            Print($"WithAsync. Options: {options}");
            var tcs = new TaskCompletionSource<object>(options);
 
            var setTask = Task.Run(
                () => {
                    Thread.Sleep(100);
                    Print("Setting task's result");
                    tcs.SetResult(null);
                    Print("Set task's result");
                });
            //await Task.Yield();
 
            await tcs.Task;

            Print("After task await");
            await setTask;
        }

        public static void Print(string msg)
        {
            Console.WriteLine($"{msg}: {Thread.CurrentThread.ManagedThreadId}");
        }
    }
}
