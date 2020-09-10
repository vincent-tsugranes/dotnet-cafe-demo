using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using dotnet.cafe.domain;

namespace dotnet.cafe.barista.Domain
{
    public class Barista
    {
        //static final Logger logger = LoggerFactory.getLogger(Barista.class);

        private string madeBy { get; set; }
        
        void setHostName() {
            try
            {
                madeBy = Environment.MachineName;
            } catch (IOException e) {
                //logger.debug("unable to get hostname");
                madeBy = "unknown";
            }
        }

        public async Task<OrderUpEvent> make(OrderInEvent orderInEvent) {

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

        private async Task<OrderUpEvent> prepare(OrderInEvent orderInEvent, int seconds) {

            try
            {
                await Task.Delay(seconds * 1000);
            }
            catch (ThreadInterruptedException e)
            {
                Thread.CurrentThread.Interrupt();
            }

            return new OrderUpEvent(
                EventType.BEVERAGE_ORDER_UP,
                orderInEvent.orderId,
                orderInEvent.name,
                orderInEvent.item,
                orderInEvent.itemId,
                madeBy);
        }        
    }
}