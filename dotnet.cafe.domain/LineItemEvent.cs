using System;

namespace dotnet.cafe.domain
{
    public class LineItemEvent
    {
        public String itemId;
        public String orderId;
        public EventType eventType;
        public String name;
        public Item item;

        public LineItemEvent() {
        }

        public LineItemEvent(EventType eventType) {
            this.eventType = eventType;
        }

        public LineItemEvent(EventType eventType, String orderId, String name, Item item, String itemId) {
            this.itemId = itemId;
            this.eventType = eventType;
            this.orderId = orderId;
            this.name = name;
            this.item = item;
        }

        public LineItemEvent(EventType eventType, String orderId, String name, Item item)
        {
            this.itemId = Guid.NewGuid().ToString();
            this.eventType = eventType;
            this.orderId = orderId;
            this.name = name;
            this.item = item;
        }       
    }
}