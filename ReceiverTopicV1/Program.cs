//https://www.rabbitmq.com/tutorials/tutorial-five-dotnet.html
using System;
using System.Linq;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
namespace ReceiverTopicV1
{
    class Program
    {
        static void ProcessMessage(string[] routingKeys)
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
                        string exchangeName = "topic_logs";
                        string queueName = channel.QueueDeclare().QueueName;
                        channel.ExchangeDeclare(exchangeName, ExchangeType.Topic);
                        foreach(string routingKey in routingKeys)
                        {
                            channel.QueueBind(queueName, exchangeName, routingKey);
                        }
                        EventingBasicConsumer consumer = new EventingBasicConsumer(channel);
                        consumer.Registered += (s, e) =>
                        {
                            Console.WriteLine($"Registered on exchange `{exchangeName}`");
                            Console.WriteLine("Press [ENTER] to quid\n");
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
            if (args.Length==0)
            {
                Console.WriteLine("Usage: ReceiverTopicV1.exe <topic1> <topic2> .... <topicN>");
                return;
            }
            ProcessMessage(args);
        }
    }
}
