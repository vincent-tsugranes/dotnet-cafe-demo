using System;
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
        
        void setHostName()
        {
            try
            {
                String hostName = Environment.MachineName;
                if (hostName.Length <= 0)
                {
                    madeBy = "default";
                }
            }
            catch (IOException e)
            {
                //logger.info("unable to get hostname; using default");
                madeBy = "unknown";
            }
        }

        public async Task<OrderUpEvent> make(OrderInEvent orderInEvent)
        {

            //logger.debug("orderIn: " + orderInEvent.toString());

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

            async Task<OrderUpEvent> prepare(OrderInEvent orderInEvent, int seconds)
            {

                try
                {
                    await Task.Delay(seconds * 1000);
                }
                catch (ThreadInterruptedException e)
                {
                    Thread.CurrentThread.Interrupt();
                }

                return new OrderUpEvent(
                    EventType.KITCHEN_ORDER_UP,
                    orderInEvent.orderId,
                    orderInEvent.name,
                    orderInEvent.item,
                    orderInEvent.itemId,
                    madeBy);

            }
        }
    }
}