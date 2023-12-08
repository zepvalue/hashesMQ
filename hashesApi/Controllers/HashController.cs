using Microsoft.AspNetCore.Mvc;
using hashesApi.Models;
using System.Security.Cryptography;
using System.Text;
using RabbitMQ.Client;


namespace hashesApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HashesController : ControllerBase
{
    private readonly ConnectionFactory _connectionFactory;
    private const string QueueName = "hashesQueue";
    private readonly ILogger<HashesController> _logger;

    public HashesController(ILogger<HashesController> logger)
    {
        // Configure RabbitMQ connection
        _connectionFactory = new ConnectionFactory
        {
            HostName = "localhost", 
            Port = 5672
        };

        _logger = logger;
    }

    [HttpGet]
    public ActionResult<Hash> GetHashes()
    {
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> GenerateAndSendHashesToQueueAsync()
    {
        try
        {
            int numberOfHashes = 4;
            List<Task> hashTasks = new List<Task>();

            for (int i = 0; i < numberOfHashes; i++)
            {
                hashTasks.Add(GenerateAndSendHashAsync());
            }

            await Task.WhenAll(hashTasks);

            return Ok("Hashes sent to RabbitMQ for further processing.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error: {ex.Message}");
        }
    }

    private async Task GenerateAndSendHashAsync()
    {
        try
        {
            // Generate a single random SHA1 hash
            using SHA1 sha1 = SHA1.Create();
            byte[] inputBytes = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString());
            byte[] hashBytes = sha1.ComputeHash(inputBytes);
            string hash = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

            await SendHashToRabbitMQAsync(hash);
        }
        catch (Exception ex)
        {
            // Handle or log any exceptions
            Console.WriteLine($"Error generating or sending hash: {ex.Message}");
        }
    }

    private Task SendHashToRabbitMQAsync(string hash)
    {
        return Task.Run(() =>
        {
            using (var connection = _connectionFactory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

                var body = Encoding.UTF8.GetBytes(hash);
                channel.BasicPublish(exchange: "", routingKey: QueueName, basicProperties: null, body: body);
            }
        });
    }

}
