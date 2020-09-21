using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using dotnet.cafe.domain;

namespace dotnet.cafe.kitchen.Domain
{
    public class Kitchen
    {
        //static final Logger logger = LoggerFactory.getLogger(Kitchen.class.getName());

        private String madeBy = "undefined";
        
        Inventory inventory;

        public Kitchen()
        {
            inventory = new Inventory();
        }
        void setHostName()
        {
            try
            {
                String hostName = Environment.MachineName;
                madeBy = hostName.Length <= 0 ? "default" : hostName;
            }
            catch (IOException e)
            {
                //logger.info("unable to get hostname; using default");
                madeBy = "unknown";
            }
        }

        public async Task<List<Event>> make(OrderInEvent orderInEvent)
        {
            setHostName();

            switch (orderInEvent.item)
            {
                case Item.CAKEPOP:
                    return await prepare(orderInEvent, 5);
                case Item.CROISSANT:
                    return await prepare(orderInEvent, 5);
                case Item.CROISSANT_CHOCOLATE:
                    return await prepare(orderInEvent, 5);
                case Item.MUFFIN:
                    return await prepare(orderInEvent, 7);
                default:
                    return await prepare(orderInEvent, 11);
            }

        }
        async Task<List<Event>> prepare(OrderInEvent orderInEvent, int seconds)
        {
            // decrement the item in inventory
            try 
            {
                inventory.decrementItem(orderInEvent.item);
            }
            catch (EightySixException e) 
            {
                Console.WriteLine(orderInEvent.item + " is 86'd");
                return new List<Event> {new EightySixEvent(orderInEvent.item)};
            }
            
            try
            {
                await Task.Delay(seconds * 1000);
            }
            catch (ThreadInterruptedException e)
            {
                Thread.CurrentThread.Interrupt();
            }

            return new List<Event>
            {
                new OrderUpEvent(
                    EventType.KITCHEN_ORDER_UP,
                    orderInEvent.orderId,
                    orderInEvent.name,
                    orderInEvent.item,
                    orderInEvent.itemId,
                    madeBy)
            };
        }
    }
}