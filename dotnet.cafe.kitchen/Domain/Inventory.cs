using System;
using System.Collections.Generic;
using System.Linq;
using dotnet.cafe.domain;

namespace dotnet.cafe.kitchen.Domain
{
    public class Inventory
    {
        static Dictionary<Item, int> stock;

        public Inventory()
        {
            createStock();
        }
        
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
        }

        public void decrementItem(Item item) {
            int currentValue = stock[item];
            if(currentValue <= 0) throw new EightySixException(item);
            stock[item] = currentValue - 1;
        }

        public Dictionary<Item, int> getStock() {
            return stock;
        }

        public int getItemCount(Item item) {
            return stock[item];
        }

        public void restock(Item item) {
            stock[item] = new Random().Next(55,99);
        } 
    }
}