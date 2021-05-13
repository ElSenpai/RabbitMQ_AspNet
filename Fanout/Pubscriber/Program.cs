using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
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

            //channel.QueueDeclare("hello-queue", true, false, false);

            //channel.ExchangeDeclare("logs-fanout", durable: true, type: ExchangeType.Fanout);

           // var randomQueueName = "log-database-save-queue";    //channel.QueueDeclare().QueueName;
            var randomQueueName = channel.QueueDeclare().QueueName;
          
           // channel.QueueDeclare(randomQueueName,true,false,false);// ilgili subs kapansa bile bu kuyruk durur.

            channel.QueueBind(randomQueueName,"logs-fanout","",null); // o nedenle bind kullandık ki silinsin

            channel.BasicQos(0, 1, false);

            var consumer = new EventingBasicConsumer(channel);

            channel.BasicConsume(randomQueueName,false, consumer);

            Console.WriteLine("loglar dinleniyor");


            consumer.Received += (object sender, BasicDeliverEventArgs e) =>
            {

                var message = Encoding.UTF8.GetString(e.Body.ToArray());

                Thread.Sleep(1500);

                Console.WriteLine("gelen mesaj:" +message);

                channel.BasicAck(e.DeliveryTag,false);
            };

            Console.ReadLine();

        }

       
    }
}
