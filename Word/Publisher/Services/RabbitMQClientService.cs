using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace Publisher.Services
{
    

    public class RabbitMQClientService
    {
        private readonly ConnectionFactory _connectionFactory;  
        private IConnection _connection;
        private IModel _channel;
        public static string exchangeName = "WordDirectExchange";
        public static string routing = "route-word";
        public static string queueName = "queue-word-file";

        public RabbitMQClientService(ConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
           
        }

        public IModel Connect()
        {
            _connection = _connectionFactory.CreateConnection();

            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(exchangeName,ExchangeType.Direct,true,false);

            _channel.QueueDeclare(queueName, true, false, false, null);

            _channel.QueueBind(queueName, exchangeName, routing);

            return _channel;

        }
    }
}
