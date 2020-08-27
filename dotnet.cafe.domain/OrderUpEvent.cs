using System;

namespace dotnet.cafe.domain
{
    public class OrderUpEvent : LineItemEvent
    {
        public String madeBy;

        public OrderUpEvent() {
        }

        public OrderUpEvent(EventType eventType, String orderId, String name, Item item, String itemId, String madeBy) 
            : base(eventType, orderId, name, item, itemId) {
            this.madeBy = madeBy;
        }
    }
}