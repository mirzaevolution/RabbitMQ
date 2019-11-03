using System;
using RabbitMQ.Client;
using System.Text;
using System.Linq;

namespace SenderV1
{
    class Program
    {
        static void SendMessage(string message)
        {
            try
            { 
                using(IConnection connection = new ConnectionFactory()
                {
                    HostName = "localhost"
                }.CreateConnection())
                {
                    using(IModel channel = connection.CreateModel())
                    {
                        channel.QueueDeclare(
                                queue: "intro",
                                durable: false,
                                exclusive: false,
                                autoDelete: false,
                                arguments: null
                            );
                        IBasicProperties basicProperties = channel.CreateBasicProperties();
                        basicProperties.Persistent = true;

                        byte[] body = Encoding.UTF8.GetBytes(message);
                        Console.WriteLine("[%] Sending message...");

                        channel.BasicPublish(
                                exchange: "",
                                routingKey: "intro",
                                basicProperties: basicProperties,
                                body: body
                            );
                        Console.WriteLine("[*] Message sent");
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
            SendMessage(string.Join(" ", args));
        }
    }
}
