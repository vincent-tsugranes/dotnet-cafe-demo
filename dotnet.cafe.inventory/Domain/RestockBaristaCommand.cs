using System;
using System.Text;
using dotnet.cafe.domain;
using dotnet.cafe.inventory.Domain.Support;

namespace dotnet.cafe.inventory.Domain
{
    public class RestockBaristaCommand : CoffeeshopCommand
    {
        private Item item { get; set; }

        int quantity { get; set; }

        public CommandType commandType = CommandType.RESTOCK_BARISTA_COMMAND;

        public RestockBaristaCommand() {
        }

        public RestockBaristaCommand(Item item) {
            this.item = item;
            this.quantity = 0;
        }

        public RestockBaristaCommand(Item item, int quantity) {
            this.item = item;
            this.quantity = quantity;
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

            if (o == null || typeof(RestockBaristaCommand) != o.GetType()) return false;

            RestockBaristaCommand that = (RestockBaristaCommand) o;

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

        public CommandType getCommandType() {
            return commandType;
        }        
    }
}