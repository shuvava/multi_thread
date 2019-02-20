using System;
using System.Threading.Tasks;

namespace producer_consumer_pattern
{
    /*
     *Output
Logger: My message
DatabaseFacade: executing 'My message'...
     *
     * TaskCompletionSource type has a very peculiar behavior: by default,
     * when SetResult method is called then all the task's "async" continuations are invoked ... synchronously
     * [lint](https://blogs.msdn.microsoft.com/seteplia/2018/10/01/the-danger-of-taskcompletionsourcet-class/)
     */
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Application started!");
            using (var facade = new DatabaseFacade())
            using (var logger = new Logger(facade))
            {
                logger.WriteLine("My message");
                await Task.Delay(100);
 
                //issue
                // to fix it look into DatabaseFacade.cs:19
                await facade.SaveAsync("Another string");
                Console.WriteLine("The string is saved");
            }

            Console.ReadKey();
        }
    }
}
