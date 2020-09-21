using System;
using System.Collections.Generic;
using dotnet.cafe.domain;

namespace dotnet.cafe.barista.Domain
{
    public class EightySixCoffeeException : Exception
    {
        public List<EightySixEvent> getEvents() {
            List<EightySixEvent> events = new List<EightySixEvent>
            {
                new EightySixEvent(Item.COFFEE_BLACK), 
                new EightySixEvent(Item.COFFEE_WITH_ROOM)
            };
            return events;
        }
    }
}