using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
namespace ReceiverV1
{
    class Program
    {

        static void ProcessMessage()
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
                        channel.BasicQos(0, 1, false);

                        channel.QueueDeclare(
                                queue: "intro",
                                durable: false,
                                exclusive: false,
                                autoDelete: false,
                                arguments: null
                            );

                        EventingBasicConsumer eventingBasicConsumer = new EventingBasicConsumer(channel);
                        eventingBasicConsumer.Registered += (s, e) =>
                        {
                            Console.WriteLine($"# Worker registered with tag: {e.ConsumerTag}");
                        };
                        eventingBasicConsumer.Received += (s, e) =>
                        {
                            string message = Encoding.UTF8.GetString(e.Body);
                            Console.WriteLine($"[*]> {message}");
                            channel.BasicAck(
                                    deliveryTag: e.DeliveryTag,
                                    multiple: false
                                );
                        };
                        channel.BasicConsume(
                                queue: "intro",
                                autoAck: false,
                                consumer: eventingBasicConsumer
                            );
                        Console.ReadLine();

                    }
                }
            }
            catch (Exception ex)
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
