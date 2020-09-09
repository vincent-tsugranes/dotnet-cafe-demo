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

        private static readonly AutoResetEvent _closingEvent = new AutoResetEvent(false);
        
        static async Task Main(string[] args)
        {
            String kafkaBootstrap = Environment.GetEnvironmentVariable("DOTNET_CAFE_KAFKA_BOOTSTRAP") ?? "127.0.0.1:9099";
            //String mongoDB = Environment.GetEnvironmentVariable("DOTNET_CAFE_MONGODB") ?? "mongodb://127.0.0.1:27017";

            KafkaService kafkaService = new KafkaService(new CafeKafkaSettings(kafkaBootstrap));
            await Task.Factory.StartNew(kafkaService.Run);
            
            Console.WriteLine("Press Ctrl + C to cancel!");
            Console.CancelKeyPress += ((s, a) =>
            {
                Console.WriteLine("Bye!");
                _closingEvent.Set();
            });
 
            _closingEvent.WaitOne();
        }
        
        
    }
}