using EDevlet.Document.Common;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EDevlet.Document.Request
{
    public partial class FrmMain : Form
    {
        IConnection _connection;
        private readonly string createDocument = "create_document_queue";
        private readonly string documentCreated = "document_created_queue";
        private readonly string documentCreateExchange = "document_create_exchange";
        IModel _channel;
        IModel channel => _channel ?? (_channel = GetChannel());
        public FrmMain()
        {
            InitializeComponent();
        }


        private void btnCreateDocument_Click(object sender, EventArgs e)
        {
            var model = new CreateDocumentModel()
            {
                UserId = 1,
                DocumentType = DocumentType.Pdf,
            };
            WriteToQueue(createDocument, model);

            var consumerEvent = new EventingBasicConsumer(channel);
            consumerEvent.Received += (ch, ea) =>
            {
                var modelRecived = JsonConvert.DeserializeObject<CreateDocumentModel>(Encoding.UTF8.GetString(ea.Body.ToArray()));
                AddLog($"Received Data Url: {modelRecived.Url}");
            };

            channel.BasicConsume(documentCreated, true, consumerEvent);

        }

        private IModel GetChannel()
        {
            return _connection.CreateModel();
        }


        private IConnection GetConnection()
        {
            var connectionFactory = new ConnectionFactory()
            {
                Uri = new Uri(txtConnectionString.Text)
            };
            return connectionFactory.CreateConnection();
        }
        private void WriteToQueue(string queueName, CreateDocumentModel model)
        {
            var messageArr = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(model));
            channel.BasicPublish(documentCreateExchange, queueName, null, messageArr);
            AddLog("Message Published");
        }

        private void txtConnectionString_TextChanged(object sender, EventArgs e)
        {
            //"amqps://wnkuwlnn:qBNamzSHcGbJW2KtDYRZ-Lmk_Mw69jLT@fish.rmq.cloudamqp.com/wnkuwlnn";

        }

        private void AddLog(string logStr)
        {
            if (txtLog.InvokeRequired)
            {
                txtLog.Invoke(new Action(() => AddLog(logStr)));
                return;
            }

            logStr = $"[{DateTime.Now:dd.MM.yyyy HH:mm:ss}] - {logStr}";
            txtLog.AppendText($"{logStr} {Environment.NewLine}");

            txtLog.SelectionStart = txtLog.Text.Length;
            txtLog.ScrollToCaret();
        }

        private void btnConnection_Click(object sender, EventArgs e)
        {
            if (_connection == null || !_connection.IsOpen)
            {
                _connection = GetConnection();
            }


            btnCreateDocument.Enabled = true;
            channel.ExchangeDeclare(documentCreateExchange, "direct");

            channel.QueueDeclare(createDocument, false, false, false);
            channel.QueueBind(createDocument, documentCreateExchange, createDocument);

            channel.QueueDeclare(documentCreated, false, false, false);
            channel.QueueBind(documentCreated, documentCreateExchange, documentCreated);

            AddLog("Connection is open now");

        }


        private void txtLog_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
