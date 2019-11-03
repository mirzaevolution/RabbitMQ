using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ReceiverExV1
{
    class Program
    {
        static void ProcessMessage()
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
                        string exchangeName = "logs";
                        channel.ExchangeDeclare(exchangeName, type: ExchangeType.Fanout);
                        string queueName = channel.QueueDeclare().QueueName;
                        channel.QueueBind(
                            queue: queueName,
                            exchange: exchangeName,
                            routingKey: "",
                            arguments: null);

                        EventingBasicConsumer consumer = new EventingBasicConsumer(channel);
                        consumer.Registered += (s, e) =>
                        {
                            Console.WriteLine($"** Worker Registered {e.ConsumerTag} **");
                            Console.WriteLine("\nPress [ENTER] to exit\n");
                        };
                        consumer.Received += (s, e) =>
                        {
                            string message = Encoding.UTF8.GetString(e.Body);
                            Console.WriteLine($"[{DateTimeOffset.Now}] > {message}");
                        };
                        channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
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
            ProcessMessage();
        }
    }
}
