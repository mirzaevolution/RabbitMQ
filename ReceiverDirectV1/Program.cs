//https://www.rabbitmq.com/tutorials/tutorial-four-dotnet.html
using System;
using System.Linq;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ReceiverDirectV1
{
    class Program
    {
        static void ProcessMessage(string severity)
        {
            try
            {
                IConnectionFactory connectionFactory = new ConnectionFactory
                {
                    HostName = "localhost"
                };
                using (IConnection connection = connectionFactory.CreateConnection())
                {
                    using(IModel channel = connection.CreateModel())
                    {
                        string exchangeName = "ex-evo-1";
                        string queueName = channel.QueueDeclare().QueueName;
                        channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
                        channel.QueueBind(queueName, exchangeName, severity);
                        EventingBasicConsumer consumer = new EventingBasicConsumer(channel);
                        consumer.Registered += (s, e) =>
                        {
                            Console.WriteLine($"Registered");
                            Console.WriteLine("Press [ENTER] to quit\n");
                        };
                        consumer.Received += (s, e) =>
                        {
                            string message = Encoding.UTF8.GetString(e.Body);
                            Console.WriteLine($"[{e.RoutingKey}]> {message}");
                            channel.BasicAck(e.DeliveryTag, false);
                        };
                        channel.BasicConsume(queueName, false, consumer);
                        Console.ReadLine();
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        static void Main(string[] args)
        {
            string[] severities = new[]
            {
                "info",
                "warning",
                "error"
            };
            if (args.Length < 1)
            {
                Console.WriteLine("Usage: ReceiverDirectV1.exe info/warning/error");
                return;
            }
            string severity = args.First();
            severity = severity.ToLower();
            if (!severities.Contains(severity))
            {

                Console.WriteLine("Usage: ReceiverDirectV1.exe info/warning/error");
                return;
            }
            ProcessMessage(severity);
        }
    }
}
