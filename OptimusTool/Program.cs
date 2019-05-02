using System;
using System.Threading.Tasks;
using Serilog;

namespace OptimusTool
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();

            await Task.Delay(3000);
            
            Console.ReadKey();
            
            Log.CloseAndFlush();
        }
    }
}