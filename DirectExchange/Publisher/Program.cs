using RabbitMQ.Client;
using System;
using System.Linq;
using System.Text;
using System.Text.Json;

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

            
            channel.ExchangeDeclare("logs-direct", durable: true, type: ExchangeType.Direct);


            Enum.GetNames(typeof(LogNames)).ToList().ForEach(x =>
            {   //senaryoya göre bu sefer publisher da kuyruk oluşturuyoruz
                var routeKey = $"route-{x}";

                var queueName = $"direct-queue-{x}";

                
                channel.QueueDeclare(queueName, true, false, false);

                channel.QueueBind(queueName, "logs-direct",routeKey, null);
            });

            Enumerable.Range(1, 50).ToList().ForEach(c =>
            {
                LogNames log = (LogNames)new Random().Next(1,5);

                string message = $"log-type: {log} ";
               // var bodyString = JsonSerializer.Serialize(message);
                var messagebody = Encoding.UTF8.GetBytes(message);

                var routeKey = $"route-{log}";

                channel.BasicPublish("logs-direct", routeKey, null, messagebody);
                Console.WriteLine($"Log gönderilmiştir : {message}");
            });




            Console.ReadLine();
        }
    }
}