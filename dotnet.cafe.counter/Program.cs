using System;
using System.Threading.Tasks;
using dotnet.cafe.domain;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace dotnet.cafe.counter
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            IConfiguration Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();
            
            var cafeDatabaseSettings = new CafeDatabaseSettings(); 
            Configuration.Bind("CafeDatabaseSettings", cafeDatabaseSettings); 
            
            //BuildWebHost(args).Run();
            LineItem item = new LineItem(item: Item.COFFEE_BLACK,"Vince");
            Console.WriteLine("Initialized: " + cafeDatabaseSettings.ConnectionString);
            
            var counter = 0;
            var max = args.Length != 0 ? Convert.ToInt32(args[0]) : -1;
            while (max == -1 || counter < max)
            {
                Console.WriteLine($"Counter: {++counter}");
                await Task.Delay(1000);
            }
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
        
        /*private static void RegisterServices()
        {
            var services = new ServiceCollection();
            services.AddSingleton<ConsoleApplication>();            
            _serviceProvider = services.BuildServiceProvider(true);
        }
        
        private static void DisposeServices()
        {
            if (_serviceProvider == null)
            {
                return;
            }
            if (_serviceProvider is IDisposable)
            {
                ((IDisposable)_serviceProvider).Dispose();
            }
        }*/
    }
}
