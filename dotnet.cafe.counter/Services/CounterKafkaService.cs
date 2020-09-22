using System;
using System.Collections;
using System.Text.Json;
using MongoDB.Driver;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using dotnet.cafe.counter.domain;
using dotnet.cafe.domain;

namespace dotnet.cafe.counter.services
{
    public class CounterKafkaService
    {
        private readonly IMongoCollection<Order> _orderRepository;
        private readonly ConsumerConfig _consumerConfig;
        private readonly ProducerConfig _producerConfig;
        public CounterKafkaService(CafeDatabaseSettings cafeDatabaseSettings, CafeKafkaSettings cafeKafkaSettings)
        {
            
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

            try
            {
                var client = new MongoClient(cafeDatabaseSettings.ConnectionString);
                var database = client.GetDatabase(cafeDatabaseSettings.DatabaseName);
                _orderRepository = database.GetCollection<Order>(cafeDatabaseSettings.OrdersCollectionName);
                
                Console.WriteLine("Read MongoDB Connection String: " + cafeDatabaseSettings.ConnectionString);
                Console.WriteLine("Read MongoDB DB Name: " + cafeDatabaseSettings.DatabaseName);
                Console.WriteLine("Read MongoDB Collection Name: " + cafeDatabaseSettings.OrdersCollectionName);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in mongodb settings: " + ex);
            }
            
        }

        public async Task Run(CancellationToken token)
        {
            Task.Run(() => { ConsumeWebInKafka(_consumerConfig, token); });
            Task.Run(() => { ConsumeOrdersOutKafka(_consumerConfig, token); });

            while (!token.IsCancellationRequested)
            {
                await Task.Delay(1 * 1000);
            }
        }
        
        void ConsumeWebInKafka(ConsumerConfig consumerConfig, CancellationToken cancellationToken)
        {
            using (var c = new ConsumerBuilder<Ignore, string>(consumerConfig).Build())
            {
                string topic = "web-in";
                c.Subscribe(topic);
                Console.WriteLine("Counter Service Listening to: " + topic);

                try
                {
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        try
                        {
                            var cr = c.Consume(cancellationToken);
                            Console.WriteLine(topic + ":" + cr.Message.Value);
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
        
        void ConsumeOrdersOutKafka(ConsumerConfig consumerConfig, CancellationToken cancellationToken)
        {
            using (var c = new ConsumerBuilder<Ignore, string>(consumerConfig).Build())
            {
                string topic = "orders-out";
                c.Subscribe(topic);
                Console.WriteLine("Counter Service Listening to: " + topic);
                try
                {
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        try
                        {
                            var cr = c.Consume(cancellationToken);
                            Console.WriteLine(topic + ":" + cr.Message.Value);
                            sendWebUpdate(cr.Message.Value);
                            
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
                    string itemString = JsonSerializer.Serialize(itemEvent);
                    var dr = await p.ProduceAsync("barista-in", new Message<Null, string> { Value = itemString });
                    sendWebUpdate(itemString);
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
                    string itemString = JsonSerializer.Serialize(itemEvent);
                    
                    var dr = await p.ProduceAsync("kitchen-in", new Message<Null, string> { Value = itemString });
                    sendWebUpdate(itemString);
                    Console.WriteLine($"Sending Order to Kitchen '{dr.Value}' to '{dr.TopicPartitionOffset}'");
                }
                catch (ProduceException<Null, string> e)
                {
                    Console.WriteLine($"Delivery failed: {e.Error.Reason}");
                }
            }
        }
        
        private async void sendWebUpdate(string itemString) {
            using (var p = new ProducerBuilder<Null, string>(_producerConfig).Build())
            {
                try
                {
                    var dr = await p.ProduceAsync("web-updates-out", new Message<Null, string> { Value = itemString });
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
            
#if (!DEBUG)
    try
    {
        await _orderRepository.InsertOneAsync(orderCreatedEvent.order);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error occured with MongoDB: {ex.Error.Reason}");
    }
#endif
            
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

