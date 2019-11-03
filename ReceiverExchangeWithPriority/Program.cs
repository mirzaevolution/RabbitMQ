using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Threading;
namespace ReceiverExchangeWithPriority
{
    class Program
    {
        private static void ProcessMessage()
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
                        channel.BasicQos(
                                prefetchSize: 0,
                                prefetchCount: 1,
                                global: false
                            );
                        string exchangeName = "x-02";
                        string routingKey = "x-route-02";
                        string queueName = "q-x-02";
                        channel.QueueDeclare(
                                queue: queueName,
                                durable: false,
                                exclusive: false,
                                autoDelete: false,
                                arguments: null
                            );
                        channel.ExchangeDeclare(
                                exchange: exchangeName,
                                type: ExchangeType.Direct
                            );
                        channel.QueueBind(
                                queue: queueName,
                                exchange: exchangeName,
                                routingKey: routingKey,
                                arguments: null
                            );
                        EventingBasicConsumer consumer = new EventingBasicConsumer(channel);
                        consumer.Registered += (s, e) =>
                        {
                            Console.WriteLine($"**** Connected to {e.ConsumerTag} ****");
                            Console.WriteLine("Press [ENTER] to quid\n");
                        };
                        consumer.Received += (s, e) =>
                        {
                            Console.WriteLine($"[%] Processing message {Convert.ToBase64String(e.Body)}");
                            string message = Encoding.UTF8.GetString(e.Body);
                            Thread.Sleep(2000);
                            Console.WriteLine($"[*] {e.DeliveryTag}: {message}");
                            channel.BasicAck(
                                    deliveryTag: e.DeliveryTag,
                                    multiple: false
                                );
                        };
                        channel.BasicConsume(
                                queue: queueName,
                                autoAck: false,
                                consumer: consumer
                            );
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
