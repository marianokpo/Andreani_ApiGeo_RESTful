using System;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Geocodificador.Librery
{
    /// <summary>
    /// RabbitMQData es una clase estatica que se otiliza como cliente para recivir y enviar informacion
    /// a travez del servicio RabbitMQ.
    /// </summary>
    class RabbitMQData
    {
        private static readonly string topic = "Geocodificador";
        private static readonly string sendTopic = "APIGEO";
        private static readonly string User = "Andreani";
        private static readonly string Pass = "GeoApi";
        private static readonly string Hostname = "localhost";
        private static readonly string Virtualhost = "/";
        private static readonly int Port = 5672;

        /// <summary>
        /// Esta funcion estatica se utiliza para enviar informacion a travez de RabbitMQ.
        /// Esta recibe un objeto de tipo LocalizadorData el cual es procesado en una cadena simple tipo csv
        /// para ser enviada.
        /// </summary>
        /// <param name="DataLD"></param>
        public static void Send(LocalizadorData DataLD)
        {

            try
            {
            string message = "ID:" +DataLD.id.ToString() + ";LAT:"+ DataLD.latitud.ToString() 
            +";LON:" + DataLD.longitud.ToString();

            var factory = new ConnectionFactory() 
            { 
                UserName = User,
                Password = Pass,
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
                    Console.WriteLine(sendTopic + " [x] Sent {0}", message);
                }
            }
            }
            catch(Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
            }
            
        }

        /// <summary>
        /// Esta funcion esttica esta diseñada para escuchar a RabbitMQ y al encontrar un mensaje
        /// ejecutar uun proceso en un thread para enviar la respuesta.
        /// </summary>
        public static void Recive()
        {
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
                        while(channel.IsOpen)
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
                                Task t = new Task(() => 
                                {
                                    ProcessInfo(message);
                                });
                                t.Start();
                            };
                            channel.BasicConsume(queue: topic,
                                                autoAck: true,
                                                consumer: consumer);
                        }
                    }
                }
            }
            catch(Exception e)
            {
                Console.WriteLine("Error RabbitMQ: " + e.Message);
            }
        }
        /// <summary>
        /// Esta funcion procesa la informacion recivida.
        /// Transforma el menjate tipo csv en el objeto LocalizadorData para luego
        /// Ejecutar un proceso de busqueda de la informacion solicitada.
        /// Luego la envia nuevamente.
        /// </summary>
        /// <param name="message"></param>
        private static void ProcessInfo(string message)
        {
            LocalizadorData LD = new LocalizadorData();
            /// <summary>
            ///  Convierte el mensaje en LocalizadorData
            /// </summary>
            foreach(string v in message.Split(";"))
            {
                string[] vr = v.Split(":");
                switch(vr[0].ToLower())
                {
                    case "id":
                        LD.id = Convert.ToInt64(vr[1]);
                        break;
                    case "ciudad":
                        LD.ciudad = vr[1];
                        break;
                    case "calle":
                        LD.calle = vr[1];
                        break;
                    case "numero":
                        LD.numero = Convert.ToInt32(vr[1]);
                        break;
                    case "codigo_postal":
                        LD.codigo_postal = vr[1];
                        break;
                    case "provincia":
                        LD.provincia = vr[1];
                        break;
                    case "pais":
                        LD.pais = vr[1];
                        break;
                    default:
                        break;
                };
            }

            if(LD != null)
            {
                /// <summary>
                /// Busca la informacion faltante.
                /// </summary>
                LD = OSM_FG.Find(LD);
                /// <summary>
                /// Envia la informacion solicitada.
                /// </summary>
                Send(LD);
            }
        }
    }
}