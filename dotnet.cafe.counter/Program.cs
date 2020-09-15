using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using dotnet.cafe.counter.services;
using dotnet.cafe.domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace dotnet.cafe.counter
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            String kafkaBootstrap = Environment.GetEnvironmentVariable("DOTNET_CAFE_KAFKA_BOOTSTRAP") ?? "127.0.0.1:9099";
            String mongoDB = Environment.GetEnvironmentVariable("DOTNET_CAFE_MONGODB") ?? "mongodb://127.0.0.1:27017";

            Console.WriteLine("Press Ctrl + C to cancel");
            Console.CancelKeyPress += (s, a) =>
            {
                tokenSource.Cancel();
                Console.WriteLine("Exiting");
            };
            
            CounterKafkaService kafkaService = new CounterKafkaService(new CafeDatabaseSettings(mongoDB), new CafeKafkaSettings(kafkaBootstrap));
            await kafkaService.Run(tokenSource.Token);
        }
    }
}
