using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using dotnet.cafe.domain;
using dotnet.cafe.kitchen.Domain;

namespace dotnet.cafe.kitchen.Services
{
    public class KitchenKafkaService
    {
        private readonly ConsumerConfig _consumerConfig;
        private readonly ProducerConfig _producerConfig;
        private readonly Kitchen _kitchen;
        
        public KitchenKafkaService(CafeKafkaSettings cafeKafkaSettings)
        {
            _kitchen = new Kitchen();
            try
            {
                _consumerConfig = new ConsumerConfig()
                {
                    GroupId = cafeKafkaSettings.GroupId,
                    BootstrapServers = cafeKafkaSettings.BootstrapServers,
                    AutoOffsetReset = AutoOffsetReset.Earliest,
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
        
        public void Run()
        {
            using (var c = new ConsumerBuilder<Ignore, string>(_consumerConfig).Build())
            {
                string topicName = "orders-in";
                c.Subscribe(topicName);
                Console.WriteLine("Subscribed to Kafka Topic: " + topicName);
                
                CancellationTokenSource cts = new CancellationTokenSource();
                
                try
                {
                    while (true)
                    {
                        try
                        {
                            var cr = c.Consume(cts.Token);
                            Console.WriteLine($"orderIn:'{cr.Message.Value}'");
                            
                            //logger.debug("\nBarista Order In Received: {}", message.getPayload());
                            OrderInEvent orderIn = JsonSerializer.Deserialize<OrderInEvent>(cr.Message.Value);

                            if (orderIn.eventType.Equals(EventType.KITCHEN_ORDER_IN)) {
                                _kitchen.make(orderIn).ContinueWith(async o =>
                                {
                                    String orderUpJson = JsonSerializer.Serialize(o);
                                    await SendMessage(orderUpJson);
                                }, cts.Token);
                            }
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

        private async Task SendMessage(String json)
        {
            using (var p = new ProducerBuilder<Null, string>(_producerConfig).Build())
            {
                try
                {   
                    var dr = await p.ProduceAsync("web-in", new Message<Null, string> { Value=json });
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