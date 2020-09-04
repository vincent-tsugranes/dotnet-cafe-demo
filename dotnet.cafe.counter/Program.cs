using System;
using System.Threading.Tasks;
using Confluent.Kafka;
using dotnet.cafe.counter.services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace dotnet.cafe.counter
{
    public class Program
    {
        private static IServiceProvider _serviceProvider;
        public static async Task Main(string[] args)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();
            
            
            RegisterServices(configuration);
            
            IServiceScope scope = _serviceProvider.CreateScope();
            scope.ServiceProvider.GetRequiredService<KafkaService>().Run();
            DisposeServices();
            
            //BuildWebHost(args).Run();
            /*LineItem item = new LineItem(item: Item.COFFEE_BLACK,"Vince");
            Console.WriteLine("Initialized: " + cafeDatabaseSettings.ConnectionString);
            
            var counter = 0;
            var max = args.Length != 0 ? Convert.ToInt32(args[0]) : -1;
            while (max == -1 || counter < max)
            {
                Console.WriteLine($"Counter: {++counter}");
                await Task.Delay(1000);
            }*/
        }

        /*public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();*/
        
        private static void RegisterServices(IConfiguration configuration)
        {
            var services = new ServiceCollection();
            services.AddSingleton<KafkaService>();
            ConfigureServices(services, configuration);
            
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
        }
        
        private static void ConfigureServices(IServiceCollection services, IConfiguration configuration )
        {


            var cafeDatabaseSettings = new CafeDatabaseSettings(); 
            configuration.Bind("CafeDatabaseSettings", cafeDatabaseSettings);
            
            services.AddSingleton<ICafeDatabaseSettings>(cafeDatabaseSettings);

            try
            {
                var kafkaSettings = configuration.GetSection("CafeKafkaSettings");
                var kafkaConsumerConfig = new ConsumerConfig()
                {
                    GroupId = kafkaSettings.GetValue<string>("GroupId"),
                    BootstrapServers = kafkaSettings.GetValue<string>("BootstrapServers"),
                    AutoOffsetReset = AutoOffsetReset.Earliest
                };
                services.AddSingleton<ConsumerConfig>(kafkaConsumerConfig);

                var kafkaProducerConfig = new ProducerConfig()
                {

                };
                services.AddSingleton<ProducerConfig>(kafkaProducerConfig);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in kafka settings: " + ex);
            }

            /*services.AddControllers()
                .AddNewtonsoftJson(options => options.UseMemberCasing());*/
        }
        
    }
}
