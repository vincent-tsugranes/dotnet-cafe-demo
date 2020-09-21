using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using dotnet.cafe.domain;

namespace dotnet.cafe.barista.Domain
{
    public class Inventory
    {
        //Logger logger = LoggerFactory.getLogger(Inventory.class.getName());

        static Dictionary<Item, int> stock;

        public Inventory()
        {
            createStock();
        }
        /*
            COFFEE_BLACK and COFFEE_WITH_ROOM are simply tracked as COFFEE_BLACK
         */
        private void createStock() {
            stock = new Dictionary<Item, int>();
            var enumValues = Enum.GetValues(typeof(Item)).Cast<Item>().ToList();
            enumValues.ForEach(value =>
            {
                stock.Add(value, new Random().Next(55,99));
            });
            foreach (KeyValuePair<Item, int> entry in stock)
            {
                Console.WriteLine(entry.Key + " " + entry.Value);
            }

            // Account for coffee
            int coffeeWithRoom = stock.Where(s => s.Key == Item.COFFEE_WITH_ROOM).Sum(s => s.Value);
            int coffeeBlack = stock.Where(s => s.Key == Item.COFFEE_BLACK).Sum(s => s.Value);
            
            stock.Remove(Item.COFFEE_BLACK);
            stock.Remove(Item.COFFEE_WITH_ROOM);
            stock.Add(Item.COFFEE_BLACK, coffeeWithRoom + coffeeBlack);
        }

        public void decrementItem(Item item) {
            if (item.Equals(Item.COFFEE_BLACK) || item.Equals(Item.COFFEE_WITH_ROOM)) {
                decrementCoffee();
            }else
            {
                int currentValue = stock[item];
                if(currentValue <= 0) throw new EightySixException(item);
                stock[item] = currentValue - 1;
            }
        }

        /*
            COFFEE_BLACK and COFFEE_WITH_ROOM are simply tracked as COFFEE_BLACK
         */
        private static void decrementCoffee()  {
            int currentValue = stock[Item.COFFEE_BLACK];
            if(currentValue <= 0) throw new EightySixCoffeeException();
            stock[Item.COFFEE_BLACK] = currentValue - 1;
        }

        public Dictionary<Item, int> getStock() {
            return stock;
        }

        public int getTotalCoffee() {
            return stock[Item.COFFEE_BLACK];
        }

        public void restock(Item item) {
            stock[item] = new Random().Next(55,99);
        }  
    }
}