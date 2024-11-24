using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System.Text;

public class MessageBusService
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(MessageBusService));
    public MessageBusService()
    {
        try
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

        }
        catch (Exception ex)
        {
            log.Debug(ex.InnerException);
            throw new ConnectFailureException("Error Could not connect", new Exception());
        }
        _channel.QueueDeclare(queue: "Workstation", durable: false, exclusive: false, autoDelete: false, arguments: null);
        _channel.QueueDeclare(queue: "AGV", durable: false, exclusive: false, autoDelete: false, arguments: null);
    }

    public void StartListening()
    {
        try
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"[x] Received: {message}");
            };
            _channel.BasicConsume(queue: "notify", autoAck: true, consumer: consumer);
        }
        catch (Exception ex)
        {
            log.Error("Could not consume the message correctly");
        }
    }

    public async Task SendMessage(JObject msg, string queue)
    {
        var routingKey = queue;
        var message = msg.ToString();
        var body = Encoding.UTF8.GetBytes(message);

        try
        {
            _channel.BasicPublish(exchange: "",
                                 routingKey: routingKey,
                                 basicProperties: null,
                                 body: body);

            log.Debug($" [x] Sent '{routingKey}':'{message}'");
        }
        catch (Exception ex)
        {
            log.Error($"Error sending message: {ex.Message}");
        }
    }

    public void Stop()
    {
        _channel.Close();
        _connection.Close();
    }
}
