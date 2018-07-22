using Cassandra;
using Cassandra.Data.Linq;
using MessageService.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MessageService.Repository
{
    public class MessageRepository : BaseCassandraRepository, IRepository<Message>
    {
        public MessageRepository(ICluster cluster) : base(cluster)
        {
            KeySpace = "demo";
        }

        public Message Delete(Guid messageId)
        {
            var localSession = GetSession();
            var messages = localSession.GetTable<Message>();

            var result = messages.DeleteIf(p => p.Id == messageId).Execute();

            return result.Existing;
        }

        public Message Get(Guid messageId)
        {
            var localSession = GetSession();
            var messages = localSession.GetTable<Message>();

            var result = messages.Where(p => p.Id == messageId).Execute().FirstOrDefault();
            if (result == null) throw new Exception($"{messageId} not found");
            return result;
        }

        public IEnumerable<Message> GetAll()
        {
            var localSession = GetSession();
            var messages = localSession.GetTable<Message>();

            var result = messages.Execute();

            return result;
        }

        public Message Insert(Message message)
        {
            var localSession = GetSession();
            var messages = localSession.GetTable<Message>();

            messages.Insert(message).Execute();

            return message;
        }

        public void Update(Guid messageId, Message message)
        {
            var localSession = GetSession();
            var messages = localSession.GetTable<Message>();

            messages.Where(p => p.Id == messageId).
                Select(p => new Message()
                {
                    Id = p.Id,
                    Text = p.Text
                })
                .Update()
                .Execute();
        }
    }
}