using Confluent.Kafka;

namespace dotnet.cafe.domain
{
    public static class KafkaConfig
    {
        public static ConsumerConfig CreateConsumerConfig(CafeKafkaSettings kafkaSettings)
        {
            return new ConsumerConfig()
            {
                GroupId = kafkaSettings.GroupId,
                BootstrapServers = kafkaSettings.BootstrapServers,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                SocketKeepaliveEnable = true,
                EnableAutoCommit = true,
                AllowAutoCreateTopics = true
            };
        }

        public static ProducerConfig CreateProducerConfig(CafeKafkaSettings kafkaSettings)
        {
            return new ProducerConfig()
            {
                BootstrapServers = kafkaSettings.BootstrapServers,
                EnableIdempotence = true
            };
        }
    }
}