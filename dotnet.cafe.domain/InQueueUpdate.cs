using System;

namespace dotnet.cafe.domain
{
    public class InQueueUpdate : WebUpdate
    {
        public InQueueUpdate() {
        }

        public InQueueUpdate(String orderId, String itemId, String name, Item item, OrderStatus status) 
            : base(orderId, itemId, name, item, status) {
        }

        public InQueueUpdate(LineItemEvent lineItemEvent) 
            : base(lineItemEvent.orderId, lineItemEvent.itemId, lineItemEvent.name, lineItemEvent.item, OrderStatus.IN_QUEUE){
            
        }
    }
}