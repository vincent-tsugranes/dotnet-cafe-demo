using System;
using System.Threading;
using System.Threading.Tasks;
using dotnet.cafe.barista.Services;
using dotnet.cafe.domain;

namespace dotnet.cafe.barista
{
    class Program
    {
        private static readonly AutoResetEvent _closingEvent = new AutoResetEvent(false);

        static async Task Main(string[] args)
        {
            String kafkaBootstrap = Environment.GetEnvironmentVariable("DOTNET_CAFE_KAFKA_BOOTSTRAP") ?? "127.0.0.1:9099";
            
            BaristaKafkaService kafkaService = new BaristaKafkaService(new CafeKafkaSettings(kafkaBootstrap));
            await Task.Factory.StartNew(kafkaService.Run);
            
            Console.WriteLine("Press Ctrl + C to cancel");
            Console.CancelKeyPress += ((s, a) =>
            {
                Console.WriteLine("Exiting");
                _closingEvent.Set();
            });
 
            _closingEvent.WaitOne();
        }
        
    }
}