using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using dotnet.cafe.barista.Domain;
using Confluent.Kafka;
using dotnet.cafe.domain;

namespace dotnet.cafe.barista.Services
{
    public class BaristaKafkaService
    {
        private readonly ConsumerConfig _consumerConfig;
        private readonly ProducerConfig _producerConfig;
        private readonly Barista _barista;
        
        public BaristaKafkaService(CafeKafkaSettings cafeKafkaSettings)
        {
            _barista = new Barista();
            try
            {
                _consumerConfig = new ConsumerConfig()
                {
                    GroupId = cafeKafkaSettings.GroupId,
                    BootstrapServers = cafeKafkaSettings.BootstrapServers,
                    AutoOffsetReset = AutoOffsetReset.Earliest,
                    SocketKeepaliveEnable = true,
                    AllowAutoCreateTopics = true
                };

                _producerConfig = new ProducerConfig()
                {
                    BootstrapServers = cafeKafkaSettings.BootstrapServers
                };
                
                Console.WriteLine("Read Kafka Bootstrap: " + cafeKafkaSettings.BootstrapServers);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in kafka settings: " + ex);
            }
        }
        
        public async Task Run(CancellationToken token)
        {
            await Task.Run(() => { ConsumeOrdersInKafka(_consumerConfig, token); }, token);
        }
        
        void ConsumeOrdersInKafka(ConsumerConfig consumerConfig, CancellationToken cancellationToken)
        {
            using (var c = new ConsumerBuilder<Ignore, string>(consumerConfig).Build())
            {
                string topicName = "orders-in";
                c.Subscribe(topicName);
                Console.WriteLine("Barista Service Listening to: " + topicName);

                try
                {
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        try
                        {
                            var cr = c.Consume(cancellationToken);
                            Console.WriteLine($"Barista Service Received" + topicName +":'{cr.Message.Value}'");
                            HandleOrderIn(cr.Message.Value, cancellationToken);

                        }
                        catch (ConsumeException e)
                        {
                            Console.WriteLine($"Error occured: {e.Error.Reason}");
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    // Ensure the consumer leaves the group cleanly and final offsets are committed.
                    c.Close();
                }
            }
        }
        private void HandleOrderIn(string message, CancellationToken cancellationToken)
        {
            OrderInEvent orderIn = JsonSerializer.Deserialize<OrderInEvent>(message);
            if (orderIn.eventType.Equals(EventType.BEVERAGE_ORDER_IN))
            {
                Console.WriteLine($"Barista Making Order " + message);
                _barista.make(orderIn).ContinueWith(async o =>
                {
                    String orderUpJson = JsonSerializer.Serialize(o.Result);
                    await SendMessage(orderUpJson);
                }, cancellationToken);
            }
        }

        private async Task SendMessage(String json)
        {
            using (var p = new ProducerBuilder<Null, string>(_producerConfig).Build())
            {
                try
                {   
                    var dr = await p.ProduceAsync("orders-out", new Message<Null, string> { Value=json });
                    Console.WriteLine($"Delivered '{dr.Value}' to '{dr.TopicPartitionOffset}'");
                }
                catch (ProduceException<Null, string> e)
                {
                    Console.WriteLine($"Delivery failed: {e.Error.Reason}");
                }
            }
        }
        
    }
}