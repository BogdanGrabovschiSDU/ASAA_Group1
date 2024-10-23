using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

public class MessageBusService
{
    private IConnection _connection;
    private IModel _channel;

    public MessageBusService()
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.QueueDeclare(queue: "workstation_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);
    }

    public void StartListening()
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine($"[x] Received: {message}");
            // Process messages from workstations or AGVs
        };
        _channel.BasicConsume(queue: "workstation_queue", autoAck: true, consumer: consumer);
    }

    public void Stop()
    {
        _channel.Close();
        _connection.Close();
    }
}
