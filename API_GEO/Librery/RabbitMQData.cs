using System;
using System.Text;
using System.Threading.Tasks;
using API_GEO.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace API_GEO.Librery
{
    class RabbitMQData
    {
        private static readonly string sendTopic = "Geocodificador";
        private static readonly string topic = "APIGEO";
        private static readonly string User = "Andreani";
        private static readonly string Pass = "GeoApi";
        private static readonly string Hostname = "rabbitmq";
        private static readonly string Virtualhost = "/";
        private static readonly int Port = 5672;
        public static async void Send(LocalizadorData DataLD)
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
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

                    //string message = "Hello World!";
                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(exchange: "",
                                        routingKey: sendTopic,
                                        basicProperties: null,
                                        body: body);
                    Console.WriteLine(" [x] Sent {0}", message);
                }
            }
        }

        public static void Recive(ApiContext _context)
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
                        channel.QueueDeclare(queue: topic,
                                            durable: false,
                                            exclusive: false,
                                            autoDelete: false,
                                            arguments: null);

                        var consumer = new EventingBasicConsumer(channel);
                        consumer.Received += (model, ea) =>
                        {
                            var body = ea.Body.ToArray();
                            message = Encoding.UTF8.GetString(body);
                            Console.WriteLine(" [x] Received {0}", message);
                            Task t = new Task(() => 
                            {
                                ProcessInfo(message,_context);
                            });
                            t.Start();
                        };
                        channel.BasicConsume(queue: topic,
                                            autoAck: true,
                                            consumer: consumer);
                    }
                }

                
            }
            catch(Exception e)
            {
                Console.WriteLine("Error RabbitMQ: " + e.Message);
            }
        }

        private static void ProcessInfo(string message, ApiContext _context)
        {
            LocalizadorData LD = new LocalizadorData();

            foreach(string v in message.Split(";"))
            {
                string[] vr = v.Split(":");
                switch(vr[0].ToLower())
                {
                    case "id":
                        LD.id = Convert.ToInt64(vr[1]);
                        break;
                    case "LAT":
                        LD.latitud = Convert.ToDouble(vr[1]);
                        break;
                    case "LON":
                        LD.longitud = Convert.ToDouble(vr[1]);
                        break;
                    default:
                        break;
                };
            }

            if(LD != null)
            {
                SQLClient SQLC = new SQLClient(_context);
                LocalizadorData lld = SQLC.GetID(LD.id);
                lld.latitud = LD.latitud;
                lld.longitud = LD.longitud;
                lld.estado = "TERMINADO";
            }
        }
    }
}