using MessageService.Models;
using System;
using System.Collections.Generic;

namespace MessageService.Services
{
    public interface IMessageService
    {
        void DeleteMessage(Guid messageId);

        IEnumerable<Message> GetAllMessages();

        Message GetMessage(Guid messageId);

        Message InsertMessage(string message);

        void UpdateMessage(Guid messageId, string message);
    }
}