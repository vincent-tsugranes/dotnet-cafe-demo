namespace dotnet.cafe.core
{
    public class CafeDatabaseSettings : ICafeDatabaseSettings
    {
        public string OrdersCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }  
    }

    public interface ICafeDatabaseSettings
    {
        string OrdersCollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }    
    }
}