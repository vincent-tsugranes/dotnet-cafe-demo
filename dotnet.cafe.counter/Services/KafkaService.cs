using System;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Confluent.Kafka;
using dotnet.cafe.counter.domain;

namespace dotnet.cafe.counter.services
{
    public class KafkaService
    {
        private readonly IMongoCollection<Order> _orders;
        
        public KafkaService(ICafeDatabaseSettings settings, ConsumerConfig consumerConfig, ProducerConfig producerConfig)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _orders = database.GetCollection<Order>(settings.OrdersCollectionName);
            

            using (var c = new ConsumerBuilder<Ignore, string>(consumerConfig).Build())
            {
                c.Subscribe("web-in");
                CancellationTokenSource cts = new CancellationTokenSource();
                try
                {
                    while (true)
                    {
                        try
                        {
                            var cr = c.Consume();
                            Console.WriteLine($"Consumed message '{cr.Value}' at: '{cr.TopicPartitionOffset}'.");
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

        
        
        /*@Incoming("web-in")
        public CompletionStage<Void> onOrderIn(final Message message) {
            this.logger.debug("orderIn: {}", message.getPayload());
            return this.handleCreateOrderCommand(JsonUtil.createOrderCommandFromJson(message.getPayload().toString())).thenRun(() -> {
                message.ack();
            });
        }

        CompletableFuture<Void> sendBaristaOrder(final LineItemEvent event) {
            return this.baristaOutEmitter.send(JsonUtil.toJson(event)).thenRun(() -> {
                this.sendWebUpdate(event);
            }).toCompletableFuture().toCompletableFuture();
        }

        CompletableFuture<Void> sendKitchenOrder(final LineItemEvent event) {
            return this.kitchenOutEmitter.send(JsonUtil.toJson(event)).thenRun(() -> {
                this.sendWebUpdate(event);
            }).toCompletableFuture();
        }

        CompletableFuture<Void> sendWebUpdate(final LineItemEvent event) {
            return this.webUpdatesOutEmitter.send(JsonUtil.toInProgressUpdate(event)).toCompletableFuture();
        }

        protected CompletionStage<Void> handleCreateOrderCommand(final CreateOrderCommand createOrderCommand) {
            OrderCreatedEvent orderCreatedEvent = Order.processCreateOrderCommand(createOrderCommand);
            this.orderRepository.persist(orderCreatedEvent.order);
            Collection<CompletableFuture<Void>> futures = new ArrayList(orderCreatedEvent.getEvents().size() * 2);
            orderCreatedEvent.getEvents().forEach((e) -> {
                if (e.eventType.equals(EventType.BEVERAGE_ORDER_IN)) {
                    futures.add(this.sendBaristaOrder(e));
                } else if (e.eventType.equals(EventType.KITCHEN_ORDER_IN)) {
                    futures.add(this.sendKitchenOrder(e));
                }

            });
            return CompletableFuture.allOf((CompletableFuture[])futures.toArray((x$0) -> {
                return new CompletableFuture[x$0];
            })).exceptionally((e) -> {
                this.logger.error(e.getMessage());
                return null;
            });
        }

        @Incoming("orders-up")
        @Outgoing("web-updates-order-up")
        public String onOrderUp(String payload) {
            this.logger.debug("received order up {}", payload);
            return JsonUtil.toDashboardUpdateReadyJson(payload);
        }*/
        
    }
}

