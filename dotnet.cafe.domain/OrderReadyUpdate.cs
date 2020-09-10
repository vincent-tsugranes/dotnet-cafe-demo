using System;
using System.Text;

namespace dotnet.cafe.domain
{
    public class OrderReadyUpdate : WebUpdate
    {
        public String madeBy { get; set; }

        public OrderReadyUpdate() {
        }

        public OrderReadyUpdate(String orderId, String itemId, String name, Item item, OrderStatus status, String madeBy) 
            : base(orderId, itemId, name, item, status) {
            this.madeBy = madeBy;
        }

        public OrderReadyUpdate(OrderUpEvent orderUpEvent) {
            this.orderId = orderUpEvent.orderId;
            this.itemId = orderUpEvent.itemId;
            this.name = orderUpEvent.name;
            this.item = orderUpEvent.item;
            this.madeBy = orderUpEvent.madeBy;
            this.status = OrderStatus.READY;
        }

        public OrderReadyUpdate(OrderInEvent orderInEvent) {
            this.orderId = orderInEvent.orderId;
            this.itemId = orderInEvent.itemId;
            this.name = orderInEvent.name;
            this.item = orderInEvent.item;
            this.status = OrderStatus.READY;
        }


        public override String ToString() {
            return new StringBuilder().
                Append("DashboardUpdate[")
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