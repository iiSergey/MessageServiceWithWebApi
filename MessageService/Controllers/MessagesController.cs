using MessageService.Models;
using MessageService.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace MessageService.Controllers
{
    [Route("api/v1/Messages")]
    public class MessagesController : Controller
    {
        private IMessageService messageService;

        public MessagesController(IMessageService messageService)
        {
            this.messageService = messageService;
        }

        /// <summary>
        /// DELETE api/v1/Messages/{id}
        /// </summary>
        /// <param name="id">Id message</param>
        [HttpDelete("{id}")]
        public void Delete(Guid id)
        {
            messageService.DeleteMessage(id);
        }

        /// <summary>
        /// GET api/v1/Messages
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<Message> Get()
        {
            return messageService.GetAllMessages();
        }

        /// <summary>
        /// GET api/v1/Messages/{id}
        /// </summary>
        /// <param name="id">Id message</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public Message Get(Guid id)
        {
            return messageService.GetMessage(id);
        }

        /// <summary>
        /// PUT api/v1/Messages
        /// </summary>
        /// <param name="text">Text message</param>
        [HttpPost]
        public void Post([FromBody]string text)
        {
            messageService.InsertMessage(text);
        }

        /// <summary>
        /// PUT api/v1/Messages/{id}
        /// </summary>
        /// <param name="id">Id message</param>
        /// <param name="text">Text message</param>
        [HttpPut("{id}")]
        public void Put(Guid id, [FromBody]string text)
        {
            messageService.UpdateMessage(id, text);
        }
    }
}