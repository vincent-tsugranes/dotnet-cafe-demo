using System;
using System.Threading;
using System.Threading.Tasks;
using dotnet.cafe.domain;

namespace dotnet.cafe.inventory.Domain
{
    public class StockRoom
    {
        //static final Logger logger = LoggerFactory.getLogger(StockRoom.class);
        private CancellationToken _cancellationToken;
        
        public async Task<CoffeeshopCommand> handleRestockItemCommand(Item item, CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;
            
            //logger.debug("restocking: {}", item);
            Console.WriteLine("restocking: {}", item);
            
            switch (item) {
                case Item.COFFEE_BLACK:
                    return await restockBarista(item, 10);
                case Item.COFFEE_WITH_ROOM:
                    return await restockBarista(item, 10);
                case Item.ESPRESSO:
                    return await restockBarista(item, 10);
                case Item.ESPRESSO_DOUBLE:
                    return await restockBarista(item, 10);
                case Item.CAPPUCCINO:
                    return await restockBarista(item, 10);
                default:
                    return await restockBarista(item, 10);
            }
        }

        /*private void sleep(int seconds) {
            // model the time to restock
            try {
                Thread.sleep(seconds * 1000);
            } catch (InterruptedException e) {
                Thread.currentThread().interrupt();
            }
        }*/

        private async Task<CoffeeshopCommand> restockBarista(Item item, int seconds)
        {
            await Task.Delay(seconds * 1000, _cancellationToken);
            return new RestockBaristaCommand(item, 99);
        }

        private async Task<CoffeeshopCommand> restockKitchen(Item item, int seconds) {
            await Task.Delay(seconds * 1000, _cancellationToken);
            return new RestockKitchenCommand(item, 99);
        }   
    }
}