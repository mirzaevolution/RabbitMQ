using System;
using System.Text;
using RabbitMQ.Client;

namespace BasicSender
{
    class Program
    {
        static void SendMessage(string message)
        {

            var factory = new ConnectionFactory()
            {
                HostName = "localhost"
            };
            using (IConnection connection = factory.CreateConnection())
            {
                using (IModel channel = connection.CreateModel())
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

                    byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                    channel.BasicPublish(exchange: "", routingKey: "intro", basicProperties: basicProperties, messageBytes);
                    Console.WriteLine($"`{message}` was sent successfully");
                }
                    
            }
            
            
        }
        static void Main(string[] args)
        {
            SendMessage(args == null||args.Length==0 ? "":args[0]);
            //Console.ReadLine();
        }
    }
}
