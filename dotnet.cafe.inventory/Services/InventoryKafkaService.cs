using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using dotnet.cafe.domain;
using dotnet.cafe.inventory.Domain;

namespace dotnet.cafe.inventory.Services
{
    public class InventoryKafkaService
    {
        private readonly ConsumerConfig _consumerConfig;
        private readonly ProducerConfig _producerConfig;
        private readonly StockRoom _stockRoom;
        
        public InventoryKafkaService(CafeKafkaSettings cafeKafkaSettings)
        {
            _stockRoom = new StockRoom();
            try
            {
                _consumerConfig = KafkaConfig.CreateConsumerConfig(cafeKafkaSettings);

                _producerConfig = KafkaConfig.CreateProducerConfig(cafeKafkaSettings);
                
                Console.WriteLine("Read Kafka Bootstrap: " + cafeKafkaSettings.BootstrapServers);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in kafka settings: " + ex);
            }
        }
        
        public async Task Run(CancellationToken token)
        {
            await Task.Run(() => { ConsumeInventoryInKafka(_consumerConfig, token); }, token);
        }
        
        void ConsumeInventoryInKafka(ConsumerConfig consumerConfig, CancellationToken cancellationToken)
        {
            using (var c = new ConsumerBuilder<Ignore, string>(consumerConfig).Build())
            {
                string topicName = "inventory-in";
                c.Subscribe(topicName);
                Console.WriteLine("Inventory Service Listening to: " + topicName);

                try
                {
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        try
                        {
                            var cr = c.Consume(cancellationToken);
                            Console.WriteLine($"Inventory Service Received " + topicName + ":" + cr.Message.Value);
                            HandleInventoryIn(cr.Message.Value, cancellationToken);

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
        private void HandleInventoryIn(string message, CancellationToken cancellationToken)
        {
            RestockItemCommand restockItemCommand = JsonSerializer.Deserialize<RestockItemCommand>(message);
            if (restockItemCommand.commandType.Equals(CommandType.RESTOCK_INVENTORY_COMMAND))
            {
                Console.WriteLine($"Inventory Received Restock Command " + message);
                _stockRoom.handleRestockItemCommand(restockItemCommand.getItem(),cancellationToken).ContinueWith(async o =>
                {
                    String coffeeshopCommand = JsonSerializer.Serialize(o.Result);
                    await SendMessage(coffeeshopCommand);
                }, cancellationToken);
            }
        }

        private async Task SendMessage(String json)
        {
            using (var p = new ProducerBuilder<Null, string>(_producerConfig).Build())
            {
                try
                {   
                    var dr = await p.ProduceAsync("inventory-out", new Message<Null, string> { Value=json });
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