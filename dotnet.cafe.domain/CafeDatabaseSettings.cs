namespace dotnet.cafe.domain
{
    public class CafeDatabaseSettings
    {
        public string OrdersCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }

        public CafeDatabaseSettings(string connectionString)
        {
            this.ConnectionString = connectionString;
            this.DatabaseName = "OrdersDb";
            this.OrdersCollectionName = "Orders";
        }
    }
}