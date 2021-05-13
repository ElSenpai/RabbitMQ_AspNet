using RabbitMQ.Client;
using System;
using System.Linq;
using System.Text;

namespace RabbitMQ.publisher
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqps://wnkuwlnn:qBNamzSHcGbJW2KtDYRZ-Lmk_Mw69jLT@fish.rmq.cloudamqp.com/wnkuwlnn");

            using var connection = factory.CreateConnection();

            var channel = connection.CreateModel();

            //channel.QueueDeclare("hello-queue", true, false, false); 
            channel.ExchangeDeclare("logs-fanout", durable: true, type: ExchangeType.Fanout);




            Enumerable.Range(1, 50).ToList().ForEach(c =>
            {
                string message =  $"log {c} ";

                var messagebody = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish("logs-fanout","", null, messagebody);
                 Console.WriteLine($"Mesaj gönderilmiştir : {message}");
            });

           

            
            Console.ReadLine();
        }
    }
}
