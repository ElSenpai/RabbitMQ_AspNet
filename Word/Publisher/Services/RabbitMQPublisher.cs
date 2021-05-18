using Publisher.Models;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Publisher.Services
{
    public class RabbitMQPublisher
    {
        private readonly RabbitMQClientService _rabbitMQClientService;

        public RabbitMQPublisher(RabbitMQClientService rabbitMQClientService)
        {
            _rabbitMQClientService = rabbitMQClientService;
        }
        public void Publish(Message message,Word word)
        {   var channel = _rabbitMQClientService.Connect();


            using (MemoryStream ms = new MemoryStream())
            {
                word.WordFile.CopyTo(ms);
                message.WordByte = ms.ToArray();
            }
            message.Email = word.Email;
            message.FileName = Path.GetFileNameWithoutExtension(word.WordFile.FileName);

            
            

            var bodyString = JsonSerializer.Serialize(message);

            var bodyByte = Encoding.UTF8.GetBytes(bodyString);

            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            channel.BasicPublish(RabbitMQClientService.exchangeName,RabbitMQClientService.routing,properties,bodyByte);

        }
    }
}
