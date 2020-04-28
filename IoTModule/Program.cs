namespace IoTModule
{
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Serilog;

    class Program
    {
        static async Task Main(string[] args)
        {
            using (var host = Host.CreateDefaultBuilder(args)
                 .ConfigureServices((hostContext, services) =>
                 {
                    services.AddSingleton<IModuleClient, ModuleClientWrapper>();
                    services.AddHostedService<EventHandlerModule>();
                 })
                 .UseSerilog()
                 .UseConsoleLifetime()
                 .Build())
            {
                await host.RunAsync();
            }
        }
    }
}
