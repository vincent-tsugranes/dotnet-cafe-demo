using System;
using System.Collections.Generic;

namespace dotnet.cafe.domain
{
    public class EightySixException : Exception
    {
        private readonly Item item;

        public EightySixException(Item item) {
            this.item = item;
        }

        public Item getItem() {
            return item;
        }

        public List<EightySixEvent> getEvents() {
            List<EightySixEvent> events = new List<EightySixEvent> {new EightySixEvent(item)};
            return events;
        }
    }
}