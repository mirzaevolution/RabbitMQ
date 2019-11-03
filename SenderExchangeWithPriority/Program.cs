using System;
using System.Text;
using RabbitMQ.Client;
namespace SenderExchangeWithPriority
{
    class Program
    {
        static void Run(string[] messages)
        {
            try
            {
                IConnectionFactory connectionFactory = new ConnectionFactory
                {
                    HostName = "localhost"
                };
                using(IConnection connection = connectionFactory.CreateConnection())
                {
                    using(IModel channel = connection.CreateModel())
                    {
                        string exchangeName = "x-02";
                        string routingKey = "x-route-02";

                        channel.ExchangeDeclare(
                               exchange: exchangeName,
                               type: ExchangeType.Direct
                            );
                        IBasicProperties basicProperties = channel.CreateBasicProperties();
                        basicProperties.Persistent = true;

                        foreach (string message in messages)
                        {
                            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                            Console.WriteLine($"[%] Sending `{message}`...");
                            channel.BasicPublish(
                                    exchange: exchangeName,
                                    routingKey: routingKey,
                                    basicProperties: basicProperties,
                                    body: messageBytes
                                );
                            Console.WriteLine($"[*] Message `{message}` has been sent");
                        }
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
            Run(args);
        }
    }
}
