using System;
using System.Threading;
using System.Threading.Tasks;
using dotnet.cafe.barista.Services;
using dotnet.cafe.domain;

namespace dotnet.cafe.barista
{
    class Program
    {
        static async Task Main(string[] args)
        {
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            String kafkaBootstrap = Environment.GetEnvironmentVariable("DOTNET_CAFE_KAFKA_BOOTSTRAP") ?? "127.0.0.1:9099";
            
            Console.WriteLine("Press Ctrl + C to cancel");
            Console.CancelKeyPress += (s, a) =>
            {
                tokenSource.Cancel();
                Console.WriteLine("Exiting");
            };
            
            BaristaKafkaService kafkaService = new BaristaKafkaService(new CafeKafkaSettings(kafkaBootstrap));
            await kafkaService.Run(tokenSource.Token);
        }
        
    }
}