using System;
using System.Text;
using dotnet.cafe.domain;
using dotnet.cafe.inventory.Domain.Support;

namespace dotnet.cafe.inventory.Domain
{
    public class RestockInventoryCommand : CoffeeshopCommand
    {
        public CommandType commandType = CommandType.RESTOCK_INVENTORY_COMMAND;

        private Item item { get; set; }

        int quantity { get; set; }

        public RestockInventoryCommand() : base() {
        }

        public RestockInventoryCommand(Item item) {
            this.item = item;
            this.quantity = 0;
        }

        public RestockInventoryCommand(Item item, int quantity) {
            this.item = item;
            this.quantity = quantity;
        }

        public override String ToString() {
            return new StringBuilder(", " + nameof(RestockInventoryCommand) + "[" + "]")
                .Append("commandType=" + commandType)
                .Append("item=" + item)
                .Append("quantity=" + quantity)
                .ToString();
        }

        public override Boolean Equals(Object o) {
            if (this == o) return true;

            if (o == null || typeof(RestockInventoryCommand) != o.GetType()) return false;

            RestockInventoryCommand that = (RestockInventoryCommand) o;

            return new EqualsBuilder()
                .Append(quantity, that.quantity)
                .Append(commandType, that.commandType)
                .Append(item, that.item)
                .IsEquals();
        }
        
        public override int GetHashCode() {
            return new HashCodeBuilder(17, 37)
                .Append(commandType)
                .Append(item)
                .Append(quantity)
                .ToHashCode();
        }

        public CommandType getCommandType() {
            return commandType;
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