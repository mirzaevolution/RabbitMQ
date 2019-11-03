using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
namespace BasicReceiver
{
    class Program
    {
        static void Run()
        {
            IConnectionFactory factory = new ConnectionFactory()
            {
                HostName = "localhost"
            };
            IConnection connection = factory.CreateConnection();
            IModel channel = connection.CreateModel();
            /* In order to change this behavior we can use the BasicQos method with the prefetchCount = 1 setting. This tells RabbitMQ not
                     * to give more than one message to a worker at a time. Or, in other words, don't dispatch a new message to a worker until it
                     * has processed and acknowledged the previous one. Instead, it will dispatch it to the next worker that is not still busy.
                     **/

            channel.BasicQos(0, 1, false);
            channel.QueueDeclare(
                    queue: "intro",
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                );
            EventingBasicConsumer consumer = new EventingBasicConsumer(channel);
            consumer.Registered += OnRegistered;
            consumer.Shutdown += OnShutdown;
            consumer.Received += (sender, eventObj) =>
            {
                string message = Encoding.UTF8.GetString(eventObj.Body);
                Console.WriteLine($"> {message}");
                channel.BasicAck(eventObj.DeliveryTag, false);
            };

            channel.BasicConsume("intro", false, consumer);
        }

       

        private static void OnShutdown(object sender, ShutdownEventArgs e)
        {
            Console.WriteLine($"Shutting down.");
        }

        private static void OnRegistered(object sender, ConsumerEventArgs e)
        {
            Console.WriteLine($"Registered. {e.ConsumerTag}");
        }

        static void Main(string[] args)
        {
            Run();
            Console.ReadLine();
        }
    }
}
