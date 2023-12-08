using System;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

public class RabbitMQReceiver
{
    private readonly string _queueName = "hashesQueue";
    private readonly string _connectionString = "amqp://guest:guest@localhost:5672/";

    public void ReceiveMessages()
    {
        var factory = new ConnectionFactory
        {
            Uri = new Uri(_connectionString)
        };

        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.QueueDeclare(queue: _queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            // Process the message (e.g., save it to the database)
            await ProcessMessageAsync(message);

            channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
        };

        channel.BasicConsume(queue: _queueName, autoAck: false, consumer: consumer);

        Console.WriteLine("Press any key to exit");
        Console.ReadLine();
    }

    private async Task ProcessMessageAsync(string sha1)
    {
        using var dbContext = new HashContext();
        var hash = new Hash { Sha1 = sha1 }; 
        dbContext.Add(hash);
        await dbContext.SaveChangesAsync();
    }
}

class Program
{
    static void Main(string[] args)
    {
        var receiver = new RabbitMQReceiver();
        receiver.ReceiveMessages();
    }
}
