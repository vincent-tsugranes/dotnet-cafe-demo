using System;
using System.Text;
using dotnet.cafe.domain;

namespace dotnet.cafe.web.Models
{
    public class DashboardUpdate
    {
        public String orderId { get; set; }

        public String itemId { get; set; }

        public String name { get; set; }

        public Item item { get; set; }

        public OrderStatus status { get; set; }
        
        public String madeBy { get; set; }
        public DashboardUpdate() {
        }
        
        public DashboardUpdate(String orderId, String name, Item item, String itemId, OrderStatus status) {
            this.orderId = orderId;
            this.itemId = itemId;
            this.name = name;
            this.item = item;
            this.status = status;
        }
        
        public DashboardUpdate(OrderUpEvent orderEvent) {
            this.orderId = orderEvent.orderId;
            this.itemId = orderEvent.itemId;
            this.name = orderEvent.name;
            this.item = orderEvent.item;
            this.madeBy = orderEvent.madeBy;
            switch (orderEvent.eventType) {
                case EventType.BEVERAGE_ORDER_IN:
                    this.status = OrderStatus.IN_QUEUE;
                    break;
                case EventType.KITCHEN_ORDER_IN:
                    this.status = OrderStatus.IN_QUEUE;
                    break;
                case EventType.BEVERAGE_ORDER_UP:
                    this.status = OrderStatus.READY;
                    break;
                case EventType.KITCHEN_ORDER_UP:
                    this.status = OrderStatus.READY;
                    break;
                default:
                    this.status = OrderStatus.IN_QUEUE;
                    break;
            }
        }
        public override String ToString() {
            return new StringBuilder().Append("DashboardUpdate[")
                .Append("orderId=")
                .Append(orderId)
                .Append(",itemId=")
                .Append(itemId)
                .Append(",name=")
                .Append(name)
                .Append(",item=")
                .Append(item)
                .Append(",status=")
                .Append(status)
                .Append("]").ToString();

        } 
    }
    
}