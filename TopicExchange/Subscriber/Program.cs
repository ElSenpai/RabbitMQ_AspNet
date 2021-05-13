using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.IO;
using System.Text;
using System.Threading;

namespace RabbitMQ.subscriber
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqps://wnkuwlnn:qBNamzSHcGbJW2KtDYRZ-Lmk_Mw69jLT@fish.rmq.cloudamqp.com/wnkuwlnn");

            using var connection = factory.CreateConnection();

            var channel = connection.CreateModel();


            channel.BasicQos(0, 1, false);

            var consumer = new EventingBasicConsumer(channel);

            var queueName = channel.QueueDeclare().QueueName; //channel dan alıcaz

            var routeKey = "Info.#"; //"*.*.Warning"

            channel.QueueBind(queueName,"logs-topic",routeKey);



            channel.BasicConsume(queueName, false, consumer);

            Console.WriteLine("loglar dinleniyor");


            consumer.Received += (object sender, BasicDeliverEventArgs e) =>
            {

                var message = Encoding.UTF8.GetString(e.Body.ToArray());

                Thread.Sleep(1500);

                Console.WriteLine("gelen mesaj:" + message);

                // File.AppendAllText("log-critical.txt", message +"\n");

                channel.BasicAck(e.DeliveryTag, false);
            };

            Console.ReadLine();

        }


    }
}