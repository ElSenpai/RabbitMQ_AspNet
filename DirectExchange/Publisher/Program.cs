using RabbitMQ.Client;
using System;
using System.Linq;
using System.Text;

namespace RabbitMQ.publisher
{
    public enum LogNames
    {
        Critical = 1,
        Error = 2,
        Warning = 3,
        Info = 4

    }


    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqps://wnkuwlnn:qBNamzSHcGbJW2KtDYRZ-Lmk_Mw69jLT@fish.rmq.cloudamqp.com/wnkuwlnn");

            using var connection = factory.CreateConnection();

            var channel = connection.CreateModel();

            //channel.QueueDeclare("hello-queue", true, false, false); 
            channel.ExchangeDeclare("logs-direct", durable: true, type: ExchangeType.Direct);


            Enum.GetNames(typeof(LogNames)).ToList().ForEach(x =>
            {   //senaryoya göre bu sefer publisher da kuyruk oluşturuyoruz
                var queueName = $"direct-queue-{x}";
                channel.QueueDeclare(queueName, true, false, false);

                channel.QueueBind(queueName, "logs-direct", null);
            });

            Enumerable.Range(1, 50).ToList().ForEach(c =>
            {


                string message = $"log {c} ";

                var messagebody = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish("logs-direct", "", null, messagebody);
                Console.WriteLine($"Mesaj gönderilmiştir : {message}");
            });




            Console.ReadLine();
        }
    }
}