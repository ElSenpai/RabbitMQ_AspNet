using EDevlet.Document.Common;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading.Tasks;

namespace EDevlet.Document.Creator
{
    class Program
    {
        static IConnection _connection;
        static private readonly string createDocument = "create_document_queue";
        static private readonly string documentCreated = "document_created_queue";
        static private readonly string documentCreateExchange = "document_create_exchange";
        static IModel _channel;
        static IModel channel => _channel ?? (_channel = GetChannel());
        static void Main(string[] args)
        {

            _connection = GetConnection();
            channel.ExchangeDeclare(documentCreateExchange, "direct");

            channel.QueueDeclare(createDocument, false, false, false);
            channel.QueueBind(createDocument, documentCreateExchange, createDocument);

            channel.QueueDeclare(documentCreated, false, false, false);
            channel.QueueBind(documentCreated, documentCreateExchange, documentCreated);

            var consumerEvent = new EventingBasicConsumer(channel);
            consumerEvent.Received += (ch, ea) =>
            {
                var modelJson = Encoding.UTF8.GetString(ea.Body.ToArray());
                var model = JsonConvert.DeserializeObject<CreateDocumentModel>(modelJson);
                Console.WriteLine ($"Received Data Url: {modelJson}");

                //Create document 
                Task.Delay(5000).GetAwaiter().GetResult();

                model.Url = "http://www.hey.com.tr/";
                WriteToQueue(documentCreated, model);

            };

            channel.BasicConsume(createDocument, true, consumerEvent);

            Console.WriteLine($"{documentCreateExchange}listening");
            Console.ReadLine();


        }
        private static IModel GetChannel()
        {
            return _connection.CreateModel();
        }


        private static IConnection GetConnection()
        {
            var connectionFactory = new ConnectionFactory()
            {
                Uri = new Uri("amqps://wnkuwlnn:qBNamzSHcGbJW2KtDYRZ-Lmk_Mw69jLT@fish.rmq.cloudamqp.com/wnkuwlnn")
            };
            return connectionFactory.CreateConnection();
        }
        private static void WriteToQueue(string queueName, CreateDocumentModel model)
        {
            var messageArr = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(model));
            channel.BasicPublish(documentCreateExchange, queueName, null, messageArr);
            Console.WriteLine("Message Published");
        }
    }
}
