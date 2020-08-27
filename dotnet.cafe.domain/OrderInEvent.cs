using System;

namespace dotnet.cafe.domain
{
    public class OrderInEvent : LineItemEvent
    {
        public OrderInEvent() {
        }

        public OrderInEvent(EventType eventType, String orderId, String name, Item item) 
            : base(eventType, orderId, name, item) {

        }

        public OrderInEvent(EventType eventType, String orderId, String itemId, String name, Item item) 
            : base(eventType, orderId, name, item) {
            
        }
    }
}