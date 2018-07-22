using MessageService.Models;
using MessageService.Repository;
using System;
using System.Collections.Generic;

namespace MessageService.Services
{
    public class MessageService : IMessageService
    {
        private readonly IRepository<Message> messageRepository;

        public MessageService(IRepository<Message> messageRepository)
        {
            this.messageRepository = messageRepository;
        }

        public void DeleteMessage(Guid messageId)
        {
            messageRepository.Delete(messageId);
        }

        public IEnumerable<Message> GetAllMessages()
        {
            return messageRepository.GetAll();
        }

        public Message GetMessage(Guid messageId)
        {
            return messageRepository.Get(messageId);
        }

        public Message InsertMessage(string message)
        {
            return messageRepository.Insert(new Message(message));
        }

        public void UpdateMessage(Guid messageId, string message)
        {
            messageRepository.Update(messageId, new Message(message));
        }
    }
}