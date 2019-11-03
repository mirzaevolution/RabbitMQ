//https://www.rabbitmq.com/tutorials/tutorial-four-dotnet.html
using System;
using System.Linq;
using System.Text;
using RabbitMQ.Client;
namespace SenderDirectV1
{
    class Program
    {
        static void SendMessage(string routingKey, string message)
        {
            try
            {
                IConnectionFactory connectionFactory = new ConnectionFactory
                {
                    HostName = "localhost"
                };
                using(IConnection connection = connectionFactory.CreateConnection())
                {
                    using (IModel channel = connection.CreateModel())
                    {
                        string exchangeName = "ex-evo-1";
                        channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
                        byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                        channel.BasicPublish(exchangeName, routingKey, null, messageBytes);
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
            if(args.Length != 2)
            {
                Console.WriteLine($"Usage: SenderDirectV1.exe info/warning/error <message>");
            }
            string severity = args.First();
            string message = args.Last();
            SendMessage(severity, message);
        }
    }
}
