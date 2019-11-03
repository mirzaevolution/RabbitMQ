using System;
using System.Text;
using RabbitMQ.Client;
namespace SenderExV1
{
    class Program
    {
        static void SendMessages(string[] messsages)
        {
            try
            {
                IConnectionFactory connectionFactory = new ConnectionFactory()
                {
                    HostName = "localhost"
                };
                using(IConnection connection = connectionFactory.CreateConnection())
                {
                    using(IModel channel = connection.CreateModel())
                    {
                        string exchangeName = "logs";
                        channel.ExchangeDeclare(exchangeName, type: ExchangeType.Fanout);
                        foreach(string message in messsages)
                        {
                            Console.WriteLine($"[%] Processing message: `{message}`");
                            channel.BasicPublish(exchangeName, "", null, Encoding.UTF8.GetBytes(message));
                            Console.WriteLine($"[*] Message sent");
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
            SendMessages(args);
        }
    }
}
