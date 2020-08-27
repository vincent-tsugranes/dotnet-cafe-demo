using System;
using System.Threading.Tasks;
using dotnet.cafe.domain;

namespace dotnet.cafe.core
{
    class Program
    {
        static async Task Main(string[] args)
        {
            LineItem item = new LineItem(item: Item.COFFEE_BLACK,"Vince");
            Console.WriteLine("Initialized: " + item.ToString());
            
            var counter = 0;
            var max = args.Length != 0 ? Convert.ToInt32(args[0]) : -1;
            while (max == -1 || counter < max)
            {
                Console.WriteLine($"Counter: {++counter}");
                await Task.Delay(1000);
            }
        }
    }
}