using System;
using System.Text;
using dotnet.cafe.domain;
using dotnet.cafe.inventory.Domain.Support;

namespace dotnet.cafe.inventory.Domain
{
    public class RestockItemCommand : CoffeeshopCommand
    {
        private Item item { get; set; }

        int quantity { get; set; }

        public CommandType commandType;

        /**
     * Default empty constructor
     */
        public RestockItemCommand() {
        }

        /**
     * Constructor for use when an item is 86'd
     *
     * @param item Item
     */
        public RestockItemCommand(Item item){
            this.item = item;
            this.quantity = 0;
        }

        /**
     *
     * @param item Item
     * @param quantity int
     */
        public RestockItemCommand(Item item, int quantity) {
            this.item = item;
            this.quantity = quantity;
        }

        public RestockItemCommand(Item item, int quantity, CommandType commandType) {
            this.item = item;
            this.quantity = quantity;
            this.commandType = commandType;
        }
        
        public override String ToString() {
            return new StringBuilder(", " + nameof(RestockBaristaCommand) + "[" + "]")
                .Append("item=" + item)
                .Append("quantity=" + quantity)
                .Append("commandType=" + commandType)
                .ToString();
        }
        
        public override Boolean Equals(Object o) {
            if (this == o) return true;
            
            if (o == null || typeof(RestockItemCommand) != o.GetType()) return false;
            
            RestockItemCommand that = (RestockItemCommand) o;

            return new EqualsBuilder()
                .Append(quantity, that.quantity)
                .Append(item, that.item)
                .Append(commandType, that.commandType)
                .IsEquals();
        }
        
        public override int GetHashCode() {
            return new HashCodeBuilder(17, 37)
                .Append(item)
                .Append(quantity)
                .Append(commandType)
                .ToHashCode();
        }

        public Item getItem() {
            return item;
        }

        public void setItem(Item item) {
            this.item = item;
        }

        public int getQuantity() {
            return quantity;
        }

        public void setQuantity(int quantity) {
            this.quantity = quantity;
        }     
    }
}