using System;
using System.Text.Json;
using dotnet.cafe.domain;

namespace dotnet.cafe.counter
{
    public static class JsonUtil
    {
        
        public static String toJson(Object obj) {
            return JsonSerializer.Serialize(obj);
        }

        public static String toDashboardUpdateReadyJson(String payload) {
            OrderUpEvent orderUpEvent = JsonSerializer.Deserialize<OrderUpEvent>(payload);
            return toJson(new OrderReadyUpdate(orderUpEvent));

        }

        public static CreateOrderCommand createOrderCommandFromJson(String payload) {
            return JsonSerializer.Deserialize<CreateOrderCommand>(payload);
        }

        public static String toInProgressUpdate(LineItemEvent lineItemEvent) {
            return toJson(new InQueueUpdate(lineItemEvent));
        }
    }
}