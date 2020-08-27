using System;
using System.Collections.Generic;
using System.Text;
using dotnet.cafe.domain;
using Microsoft.Extensions.Logging;

namespace dotnet.cafe.counter.domain
{
    public class Order
    {
        
    //    @Transient
    //    static final Logger logger = LoggerFactory.getLogger(Order.class);
    //    @BsonId
        public String id;

        public List<LineItem> beverageLineItems = new List<LineItem>();

        public List<LineItem> kitchenLineItems = new List<LineItem>();

        public Order() {
        }

        public Order(List<LineItem> beverageLineItems) {
            this.beverageLineItems = beverageLineItems;
        }

        public static OrderCreatedEvent processCreateOrderCommand(CreateOrderCommand createOrderCommand) {
            Order order = createOrderFromCommand(createOrderCommand);
            return createOrderCreatedEvent(order);
        }

        /*
            Creates the Value Objects associated with a new Order
         */
        private static OrderCreatedEvent createOrderCreatedEvent(Order order) {
            // construct the OrderCreatedEvent
            OrderCreatedEvent orderCreatedEvent = new OrderCreatedEvent();
            orderCreatedEvent.order = order;
            
            order.beverageLineItems?.ForEach(b =>
            {
                orderCreatedEvent.addEvent(new OrderInEvent(EventType.BEVERAGE_ORDER_IN, order.id, b.name, b.item));
            });
            
            order.kitchenLineItems?.ForEach(k =>
            {
                orderCreatedEvent.addEvent(new OrderInEvent(EventType.KITCHEN_ORDER_IN, order.id, k.name, k.item));
            });
            
            //logger.debug("createEventFromCommand: returning OrderCreatedEvent {}", orderCreatedEvent.toString());
            return orderCreatedEvent;
        }

        private static Order createOrderFromCommand(CreateOrderCommand createOrderCommand) {
            //logger.debug("createOrderFromCommand: CreateOrderCommand {}", createOrderCommand.toString());

            // build the order from the CreateOrderCommand
            Order order = new Order();
            order.id = createOrderCommand.id;
            
            createOrderCommand.beverages.ForEach(b =>
            {
                //logger.debug("createOrderFromCommand adding beverage {}", b.toString());
                order.getBeverageLineItems().Add(new LineItem(b.item, b.name));
            });

            createOrderCommand.kitchenOrders.ForEach(k =>
            {
                //logger.debug("createOrderFromCommand adding kitchenOrder {}", k.toString());
                order.getKitchenLineItems().Add(new LineItem(k.item, k.name));
            });
            
            return order;
        }

        public List<LineItem> getBeverageLineItems() {
            return beverageLineItems;
        }

        public List<LineItem> getKitchenLineItems() {
            return kitchenLineItems;
        }
        
        public override String ToString() {
            return new StringBuilder()
                    .Append("id:" + this.id)
                    .Append("beverageLineItems: " + beverageLineItems)
                    .Append("kitchenLineItems: " + kitchenLineItems)
                    .ToString();
        }
    }
}