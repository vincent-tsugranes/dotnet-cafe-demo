using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using dotnet.cafe.domain;
using dotnet.cafe.kitchen.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace dotnet.cafe.kitchen
{
    class Program
    {

        static async Task Main(string[] args)
        {
            var tokenSource = new CancellationTokenSource();
            String kafkaBootstrap = Environment.GetEnvironmentVariable("DOTNET_CAFE_KAFKA_BOOTSTRAP") ?? "127.0.0.1:9099";

            Console.WriteLine("Press Ctrl + C to cancel");
            Console.CancelKeyPress += (s, a) =>
            {
                tokenSource.Cancel();
                Console.WriteLine("Exiting");
            };
            
            KitchenKafkaService kafkaService = new KitchenKafkaService(new CafeKafkaSettings(kafkaBootstrap));
            await kafkaService.Run(tokenSource.Token);
        }
        
        
    }
}