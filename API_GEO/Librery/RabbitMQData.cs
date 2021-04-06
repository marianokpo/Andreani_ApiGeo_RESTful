using System;
using System.Text;
using System.Threading.Tasks;
using API_GEO.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace API_GEO.Librery
{
    /// <summary>
    /// clase de comunicacion con el servicio RabbitMQ
    /// </summary>
    class RabbitMQData
    {
        private static readonly string sendTopic = "Geocodificador";
        private static readonly string topic = "APIGEO";
        private static readonly string User = "Andreani";
        private static readonly string Pass = "GeoApi";
        private static readonly string Hostname = "localhost";
        private static readonly string Virtualhost = "/";
        private static readonly int Port = 5672;

        /// <summary>
        /// Se utiliza para enviar informacion entre las api con el servicio RabbitMQ
        /// </summary>
        /// <param name="DataLD"></param>
        public static void Send(LocalizadorData DataLD)
        {
            string message = "id:" +DataLD.id.ToString() + ";calle:"+ DataLD.calle 
            +":numero:" + DataLD.numero.ToString() + ";ciudad:" + DataLD.ciudad
            + ";pais:" + DataLD.pais + ";provincia:" + DataLD.provincia
            + ";codigo_postal:" + DataLD.codigo_postal;
            
            var factory = new ConnectionFactory() 
            { 
                UserName = User,
                Password = Pass,
                VirtualHost = Virtualhost,
                HostName = Hostname,
                Port = Port
            };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                     channel.QueueDeclare(queue: sendTopic,
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

                    var body = Encoding.UTF8.GetBytes(message);

                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;

                    channel.BasicPublish(exchange: "",
                                        routingKey: sendTopic,
                                        basicProperties: properties,
                                        body: body);
                    Console.WriteLine(" [x] Sent {0}", message);
                }
            }
        }
        /// <summary>
        /// Se utiliza para escuchar y recivir los mensajes de RabbitMQ.
        /// </summary>
        /// <param name="_context"></param>
        /// <returns>messaje</returns>
        public static string Recive(ApiContext _context)
        {
            Console.WriteLine("Received to {0}", topic);
            try
            {
                string message = null;

                var factory = new ConnectionFactory() 
                { 
                    UserName = User,
                    Password = Pass,
                    VirtualHost = Virtualhost,
                    HostName = Hostname,
                    Port = Port
                };

                using(var connection = factory.CreateConnection())
                {
                    using(var channel = connection.CreateModel())
                    {
                        while(channel.IsOpen && message == null)
                        {
                            channel.QueueDeclare(queue: topic,
                                                durable: true,
                                                exclusive: false,
                                                autoDelete: false,
                                                arguments: null);
                            
                            channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
                            
                            var consumer = new EventingBasicConsumer(channel);
                            consumer.Received += (model, ea) =>
                            {
                                var body = ea.Body.ToArray();
                                message = Encoding.UTF8.GetString(body);
                                Console.WriteLine(" [x] Received {0}", message);
                            };
                            channel.BasicConsume(queue: topic,
                                                autoAck: true,
                                                consumer: consumer);
                        }
                    }
                }
                return message;
                
            }
            catch(Exception e)
            {
                Console.WriteLine("Error RabbitMQ: " + e.Message);
                return "";
            }
        }
    }
}