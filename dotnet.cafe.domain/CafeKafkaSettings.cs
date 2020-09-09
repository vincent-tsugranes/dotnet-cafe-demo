namespace dotnet.cafe.domain
{
    public class CafeKafkaSettings
    {
        public string BootstrapServers { get; set; }
        public string GroupId { get; set; }

        public CafeKafkaSettings(string bootstrapServers)
        {
            this.BootstrapServers = bootstrapServers;
            this.GroupId = "cafe-group";
        }
    }
}