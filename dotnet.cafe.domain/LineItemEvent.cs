using System;

namespace dotnet.cafe.domain
{
    public class LineItemEvent : Event
    {
        public String itemId { get; set; }
        public String orderId { get; set; }
        public EventType eventType { get; set; }
        public String name { get; set; }
        public Item item { get; set; }

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

        public EventType getEventType()
        {
            return this.eventType;
        }
    }
}