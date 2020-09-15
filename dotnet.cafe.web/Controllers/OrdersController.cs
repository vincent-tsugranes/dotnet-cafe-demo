using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Confluent.Kafka;
using dotnet.cafe.domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace dotnet.cafe.web.Controllers
{

    [Route("/api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {

        [HttpGet]
        public string Get(int id)
        {
            return "Order " + id;
        }
        
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<HttpResponseMessage> Post([FromBody] CreateOrderCommand createOrderCommand)
        {
            HttpResponseMessage returnMessage;
            try
            {
                returnMessage = new HttpResponseMessage(HttpStatusCode.Created);

                //logger.debug("CreateOrderCommand received: {}", toJson(createOrderCommand));
                String orderReceivedJson = JsonSerializer.Serialize(createOrderCommand);
                returnMessage.RequestMessage = new HttpRequestMessage(HttpMethod.Post, orderReceivedJson);
                await SendMessage(orderReceivedJson);
            }
            catch (Exception ex)
            {
                returnMessage = new HttpResponseMessage(HttpStatusCode.ExpectationFailed)
                {
                    RequestMessage = new HttpRequestMessage(HttpMethod.Post, ex.ToString())
                };
            }
            return await Task.FromResult(returnMessage);
        }
        
        private async Task SendMessage(String json)
        {
            String kafkaBootstrap = Environment.GetEnvironmentVariable("DOTNET_CAFE_KAFKA_BOOTSTRAP") ?? "127.0.0.1:9099";
            var cafeKafkaSettings = new CafeKafkaSettings(kafkaBootstrap);
            var producerConfig = new ProducerConfig { BootstrapServers = cafeKafkaSettings.BootstrapServers };
            using (var p = new ProducerBuilder<Null, string>(producerConfig).Build())
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