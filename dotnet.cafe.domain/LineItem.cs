using System;
using System.Text;

namespace dotnet.cafe.domain
{
    public class LineItem
    {
        public Item item;

        public String name;

        public LineItem(Item item, String name) {
            this.item = item;
            this.name = name;
        }

        public LineItem() {
        }

        public override String ToString() {
            return new StringBuilder()
                .Append("item:" + item)
                .Append("name:" + name)
                .ToString();
        }       
    }
}