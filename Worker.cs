using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Configuration;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace GerenciamentoTarefasWorker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private IConnection _connection;
        private IModel _channel;
        private readonly RabbitMQOptions _rabbitMQOptions;

        public Worker(ILogger<Worker> logger, IConfiguration configuration)
        {
            _logger = logger;
            _rabbitMQOptions = configuration.GetSection("RabbitMQ").Get<RabbitMQOptions>();
            InitRabbitMQ();
        }

        private void InitRabbitMQ()
        {
            var factory = new ConnectionFactory
            {
                HostName = _rabbitMQOptions.HostName,
                Port = _rabbitMQOptions.Port,
                UserName = _rabbitMQOptions.UserName,
                Password = _rabbitMQOptions.Password
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(queue: "task_queue",
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            _logger.LogInformation("RabbitMQ conexao estabelecida.");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                // Aqui você processa a mensagem recebida
                _logger.LogInformation($"[x] Received {message}");

                // Confirmação de recebimento
                _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            };

            _channel.BasicConsume(queue: "task_queue",
                                 autoAck: false, // habilita confirmação manual de mensagem
                                 consumer: consumer);

            await Task.CompletedTask;
        }

        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }
    }
}

