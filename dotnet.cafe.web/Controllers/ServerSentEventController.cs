using System;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using dotnet.cafe.domain;
using dotnet.cafe.web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace dotnet.cafe.web.Controllers
{
    [Route("/api/sse")]
    [ApiController]
    public class ServerSentEventController : ControllerBase
    {
        
        [HttpGet]
        public async Task Get(CancellationToken cancellationToken)
        {
            var response = Response;
            response.StatusCode = 200;
            response.Headers.Add("Content-Type", "text/event-stream");
            response.Headers.Add("Cache-Control", "no-cache");
            response.Headers.Add("Connection", "keep-alive");
            
            ConcurrentQueue<string> queue = new ConcurrentQueue<string>();
            
            String kafkaBootstrap = Environment.GetEnvironmentVariable("DOTNET_CAFE_KAFKA_BOOTSTRAP") ?? "127.0.0.1:9099";
            var cafeKafkaSettings = new CafeKafkaSettings(kafkaBootstrap);
            var consumerConfig = new ConsumerConfig()
            {
                GroupId = cafeKafkaSettings.GroupId,
                BootstrapServers = cafeKafkaSettings.BootstrapServers,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                SocketKeepaliveEnable = true,
                AllowAutoCreateTopics = true
            };
            
            using (var c = new ConsumerBuilder<Ignore, string>(consumerConfig).Build())
            {
                c.Subscribe("web-updates-out");
                Task.Run(() =>
                {
                    try
                    {
                        while (true)
                        {
                            try
                            {
                                var cr = c.Consume(cancellationToken);
                                var messageValue = cr.Message.Value;
                                Console.WriteLine("Received message on web-updates-out: " + messageValue);
                                queue.Enqueue(messageValue);
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
                });
                
                await response.Body.FlushAsync();
                
                for(var i = 0; true; ++i)
                {
                    while (queue.TryDequeue(out var message))
                    {
                        LineItemEvent item = JsonSerializer.Deserialize<LineItemEvent>(message);
                        DashboardUpdate dashboardUpdate = new DashboardUpdate(item);

                        var serializerOptions = new JsonSerializerOptions
                        {
                            Converters = {new JsonStringEnumConverter()},
                            IgnoreNullValues = true
                        };
                    
                        String dashboardUpdateJson = JsonSerializer.Serialize(dashboardUpdate, serializerOptions);
                        await response.WriteAsync($"data:{dashboardUpdateJson} \n\n");
                        await response.Body.FlushAsync();
                    }

                    await Task.Delay(1 * 1000 );
                }
            }

        }
    }
}