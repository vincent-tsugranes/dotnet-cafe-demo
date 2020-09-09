using System;
using System.Collections;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using dotnet.cafe.counter.domain;
using dotnet.cafe.domain;
using MongoDB.Bson;

namespace dotnet.cafe.counter.services
{
    public class KafkaService
    {
        public CancellationTokenSource cts = new CancellationTokenSource();
        
        private readonly IMongoCollection<Order> _orderRepository;
        private readonly ConsumerConfig _consumerConfig;
        private readonly ProducerConfig _producerConfig;
        public KafkaService(CafeDatabaseSettings cafeDatabaseSettings, CafeKafkaSettings cafeKafkaSettings)
        {
            
            try
            {
                _consumerConfig = new ConsumerConfig()
                {
                    GroupId = cafeKafkaSettings.GroupId,
                    BootstrapServers = cafeKafkaSettings.BootstrapServers,
                    AutoOffsetReset = AutoOffsetReset.Earliest
                };

                _producerConfig = new ProducerConfig()
                {
                    BootstrapServers = cafeKafkaSettings.BootstrapServers
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in kafka settings: " + ex);
            }

            try
            {
                var client = new MongoClient(cafeDatabaseSettings.ConnectionString);
                var database = client.GetDatabase(cafeDatabaseSettings.DatabaseName);
                _orderRepository = database.GetCollection<Order>(cafeDatabaseSettings.OrdersCollectionName);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in mongodb settings: " + ex);
            }
            
        }

        public void Run()
        {
            using (var c = new ConsumerBuilder<Ignore, string>(_consumerConfig).Build())
            {
                c.Subscribe("web-in");
                
                try
                {
                    while (true)
                    {
                        try
                        {
                            var cr = c.Consume(cts.Token);
                            Console.WriteLine($"orderIn:'{cr.Message.Value}'");
                            CreateOrderCommand orderCommand = JsonUtil.createOrderCommandFromJson(cr.Message.Value);
                            handleCreateOrderCommand(orderCommand);
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
        
        private async void sendBaristaOrder(LineItemEvent itemEvent) {
            using (var p = new ProducerBuilder<Null, string>(_producerConfig).Build())
            {
                try
                {
                    var dr = await p.ProduceAsync("barista-out", new Message<Null, string> { Value = itemEvent.ToJson() });
                    sendWebUpdate(itemEvent);
                    Console.WriteLine($"Sending Order to Barista '{dr.Value}' to '{dr.TopicPartitionOffset}'");
                }
                catch (ProduceException<Null, string> e)
                {
                    Console.WriteLine($"Delivery failed: {e.Error.Reason}");
                }
            }
        }

        private async void sendKitchenOrder(LineItemEvent itemEvent) {
            using (var p = new ProducerBuilder<Null, string>(_producerConfig).Build())
            {
                try
                {
                    var dr = await p.ProduceAsync("kitchen-out", new Message<Null, string> { Value = itemEvent.ToJson() });
                    sendWebUpdate(itemEvent);
                    Console.WriteLine($"Sending Order to Kitchen '{dr.Value}' to '{dr.TopicPartitionOffset}'");
                }
                catch (ProduceException<Null, string> e)
                {
                    Console.WriteLine($"Delivery failed: {e.Error.Reason}");
                }
            }
        }
        
        private async void sendWebUpdate(LineItemEvent itemEvent) {
            using (var p = new ProducerBuilder<Null, string>(_producerConfig).Build())
            {
                try
                {
                    var dr = await p.ProduceAsync("web-updates-out", new Message<Null, string> { Value = itemEvent.ToJson() });
                    Console.WriteLine($"Sending Order to Web '{dr.Value}' to '{dr.TopicPartitionOffset}'");
                }
                catch (ProduceException<Null, string> e)
                {
                    Console.WriteLine($"Delivery failed: {e.Error.Reason}");
                }
            }
        }
        
        private async void handleCreateOrderCommand(CreateOrderCommand createOrderCommand) {
            OrderCreatedEvent orderCreatedEvent = Order.processCreateOrderCommand(createOrderCommand);
            await _orderRepository.InsertOneAsync(orderCreatedEvent.order);
            orderCreatedEvent.getEvents().ForEach(e =>
            {
                if (e.eventType == EventType.BEVERAGE_ORDER_IN)
                {
                    sendBaristaOrder(e);
                }else if (e.eventType == EventType.KITCHEN_ORDER_IN)
                {
                    sendKitchenOrder(e);
                }
            }); 
        }

    }
}

