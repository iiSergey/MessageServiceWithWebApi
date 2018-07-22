using System;

namespace MessageService.Models
{
    public class Message
    {
        public Message(string text)
        {
            Id = Guid.NewGuid();
            Text = text;
        }

        public Message()
        {
        }

        public Guid Id { get; set; }
        public string Text { get; set; }
    }
}