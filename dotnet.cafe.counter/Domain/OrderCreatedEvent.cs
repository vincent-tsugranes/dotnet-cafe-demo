using System.Collections;
using System.Collections.Generic;
using dotnet.cafe.domain;

namespace dotnet.cafe.counter.domain
{
    public class OrderCreatedEvent
    {
        public Order order;
        public List<LineItemEvent> events = new List<LineItemEvent>();

        public OrderCreatedEvent() {
        }

        public void addEvent(LineItemEvent orderEvent) {
            this.getEvents().Add(orderEvent);
        }

        public List<LineItemEvent> getEvents() {
            if (this.events == null) {
                this.events = new List<LineItemEvent>();
            }

            return this.events;
        }

        public void setOrder(Order order) {
            this.order = order;
        }

        public void addEvents(List<LineItemEvent> orderEvents) {
            this.getEvents().AddRange(orderEvents);
        }      
    }
}