using System;
using System.Threading;
using System.Threading.Tasks;
using dotnet.cafe.domain;
using dotnet.cafe.inventory.Services;

namespace dotnet.cafe.inventory
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            String kafkaBootstrap = Environment.GetEnvironmentVariable("DOTNET_CAFE_KAFKA_BOOTSTRAP") ?? "127.0.0.1:9099";

            Console.WriteLine("Press Ctrl + C to cancel");
            Console.CancelKeyPress += (s, a) =>
            {
                tokenSource.Cancel();
                Console.WriteLine("Exiting");
            };
            
            InventoryKafkaService kafkaService = new InventoryKafkaService(new CafeKafkaSettings(kafkaBootstrap));
            await kafkaService.Run(tokenSource.Token);
        }
    }
}