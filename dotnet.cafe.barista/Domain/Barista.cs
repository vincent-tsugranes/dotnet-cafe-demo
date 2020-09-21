using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using dotnet.cafe.domain;

namespace dotnet.cafe.barista.Domain
{
    public class Barista
    {
        //static final Logger logger = LoggerFactory.getLogger(Barista.class);

        private String madeBy = "undefined";
        
        Inventory inventory;

        public Barista()
        {
            inventory = new Inventory();
        }
        void setHostName()
        {
            try
            {
                String hostName = System.Net.Dns.GetHostName();
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
            
            //logger.debug("orderIn: " + orderInEvent.toString());

            switch(orderInEvent.item){
                    case Item.COFFEE_BLACK:
                        return await prepare(orderInEvent, 5);
                    case Item.COFFEE_WITH_ROOM:
                        return await prepare(orderInEvent, 5);
                    case Item.ESPRESSO:
                        return await prepare(orderInEvent, 7);
                    case Item.ESPRESSO_DOUBLE:
                        return await prepare(orderInEvent, 7);
                    case Item.CAPPUCCINO:
                        return await prepare(orderInEvent, 9);
                    default:
                        return await prepare(orderInEvent, 11);
                }
        }

        private async Task<List<Event>> prepare(OrderInEvent orderInEvent, int seconds) {

            // decrement the item in inventory
            try 
            {
                inventory.decrementItem(orderInEvent.item);
            } 
            catch (EightySixException e) {
                Console.WriteLine(orderInEvent.item + " is 86'd");
                return new List<Event> {new EightySixEvent(orderInEvent.item)};
            } 
            catch (EightySixCoffeeException e) {
                // 86 both coffee items
                Console.WriteLine("coffee is 86'd");
                return new List<Event>
                {
                    new EightySixEvent(Item.COFFEE_WITH_ROOM),
                    new EightySixEvent(Item.COFFEE_BLACK)
                };
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
                    EventType.BEVERAGE_ORDER_UP,
                    orderInEvent.orderId,
                    orderInEvent.name,
                    orderInEvent.item,
                    orderInEvent.itemId,
                    madeBy)
            };
        }        
    }
}