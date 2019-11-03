//https://www.rabbitmq.com/tutorials/tutorial-five-dotnet.html
using System;
using System.Linq;
using System.Text;
using RabbitMQ.Client;
namespace SenderTopicV1
{
    class Program
    {
        static void SendMessage(string topic, string message)
        {
            try
            {
                IConnectionFactory factory = new ConnectionFactory
                {
                    HostName = "localhost"
                };
                using(IConnection connection = factory.CreateConnection())
                {
                    using(IModel channel = connection.CreateModel())
                    {
                        string exchangeName = "topic_logs";

                        channel.ExchangeDeclare(exchangeName, ExchangeType.Topic);

                        byte[] data = Encoding.UTF8.GetBytes(message);

                        channel.BasicPublish(exchangeName, topic, null, data);

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
            if (args.Length != 2)
            {
                Console.WriteLine($"Usage: SenderTopicV1.exe <topic> <message>");
            }
            string severity = args.First();
            string message = args.Last();
            SendMessage(severity, message);
        }
    }
}
