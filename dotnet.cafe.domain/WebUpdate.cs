using System;

namespace dotnet.cafe.domain
{
    public abstract class WebUpdate
    {   
        public String orderId { get; set; }
        public String itemId { get; set; }
        public String name { get; set; }
        public Item item { get; set; }
        public OrderStatus status { get; set; }

        public WebUpdate() {
        }

        public WebUpdate(String orderId, String itemId, String name, Item item, OrderStatus status) {
            this.orderId = orderId;
            this.itemId = itemId;
            this.name = name;
            this.item = item;
            this.status = status;
        }
        
    }
}