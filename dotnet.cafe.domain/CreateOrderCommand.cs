using System;
using System.Collections.Generic;
using System.Text;

namespace dotnet.cafe.domain
{
    public class CreateOrderCommand
    {
        public String id;

        public List<LineItem> beverages = new List<LineItem>();

        public List<LineItem> kitchenOrders = new List<LineItem>();

        public CreateOrderCommand() {
        }

        public CreateOrderCommand(String id, List<LineItem> beverages, List<LineItem> kitchenOrders) {
            this.id = id;
            this.beverages = beverages;
            this.kitchenOrders = kitchenOrders;
        }

        public List<LineItem> getBeverages() {
            //Java
            //return beverages == null ? new ArrayList<LineItem>() : beverages;
            //Use null coalescing in C#
            return beverages ?? new List<LineItem>();
        }

        public List<LineItem> getKitchenOrders() {
            return kitchenOrders ?? new List<LineItem>();
        }

        public void addBeverages(String id, List<LineItem> beverageList) {
            this.id = id;
            this.beverages.AddRange(beverageList);
        }

        public void addKitchenItems(String id, List<LineItem> kitchenOrdersList) {
            this.id = id;
            this.kitchenOrders.AddRange(kitchenOrdersList);
        }

        
        public override string ToString() {
            return new StringBuilder()
                .Append("id:" + id)
                .Append("beverages:" + beverages)
                .Append("kitchenOrders:" + kitchenOrders)
                .ToString();
        }        
    }
}