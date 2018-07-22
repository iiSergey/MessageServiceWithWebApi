using Cassandra.Mapping;
using MessageService.Models;

namespace MessageService
{
    public class MessageMappings : Mappings
    {
        public MessageMappings()
        {
            For<Message>()
                .TableName("messages")
                .PartitionKey(u => u.Id)
                .Column(u => u.Id, cm => cm.WithName("id"))
                .Column(u => u.Text, cm => cm.WithName("textMessage"));
        }
    }
}