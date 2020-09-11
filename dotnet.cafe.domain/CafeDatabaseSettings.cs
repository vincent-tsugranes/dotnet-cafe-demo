namespace dotnet.cafe.domain
{
    public class CafeDatabaseSettings
    {
        public string OrdersCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }

        public CafeDatabaseSettings(string connectionString, string databaseName = "OrdersDB", string ordersCollectionName = "Orders")
        {
            this.ConnectionString = connectionString;
            this.DatabaseName = databaseName;
            this.OrdersCollectionName = ordersCollectionName;
        }
    }
}