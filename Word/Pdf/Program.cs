

using RabbitMQ.Client;
using RabbitMQ.Client.Events;


using Spire.Doc;
using System;
using System.IO;
using System.Net.Mail;
using System.Text;
using System.Text.Json;

namespace Pdf
{
    class Program
    {
        private static IModel _channel;
        public static string exchangeName = "WordDirectExchange";
        public static string routing = "route-word";
        public static string queueName = "queue-word-file";
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqps://gedkdugy:6Z4QQPh8FiuRNGYHNv7HSCQR4HS1J_Io@hippo.rmq2.cloudamqp.com/gedkdugy");

            using var connection = factory.CreateConnection();
            _channel = connection.CreateModel();
            _channel.BasicQos(0, 1, false);
            var consumer = new EventingBasicConsumer(_channel);
            SubscribeForPdf(consumer);

        }
        public static void SubscribeForPdf(EventingBasicConsumer consumer)
        {
            bool result = false;

            _channel.QueueBind(queueName, exchangeName, routing);
            _channel.BasicConsume(queueName, false, consumer);

            consumer.Received += (object sender, BasicDeliverEventArgs e) =>
            {
                try
                {
                    Document document = new Document();

                    var messageStr = Encoding.UTF8.GetString(e.Body.ToArray());
                    Message message = JsonSerializer.Deserialize<Message>(messageStr);
                    document.LoadFromStream(new MemoryStream(message.WordByte), FileFormat.Docx2013);

                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        EmailService emailSender = new EmailService();
                        document.SaveToStream(memoryStream, FileFormat.PDF);
                        result = emailSender.EmailSend(message.Email, memoryStream, message.FileName);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occured: {ex.Message}");
                }
                if (result)
                {
                    Console.WriteLine("The message in the queue has been processed successfully.");
                    _channel.BasicAck(e.DeliveryTag, false);
                }
            };
            Console.ReadLine();
        }

       
    }
}