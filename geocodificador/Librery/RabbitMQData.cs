using System;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Geocodificador.Librery
{
    class RabbitMQData
    {
        private static readonly string topic = "Geocodificador";
        private static readonly string sendTopic = "APIGEO";
        private static readonly string User = "Andreani";
        private static readonly string Pass = "GeoApi";
        private static readonly string Hostname = "rabbitmq";
        private static readonly string Virtualhost = "/";
        private static readonly int Port = 5672;
        public static async void Send(LocalizadorData DataLD)
        {

            string message = "ID:" +DataLD.id.ToString() + ";LAT:"+ DataLD.latitud.ToString() 
            +":LON:" + DataLD.longitud.ToString();

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
            catch(Exception e)
            {
                Console.WriteLine("Error RabbitMQ: " + e.Message);
            }
        }

        private static void ProcessInfo(string message)
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
                LD = OSM_FG.Find(LD);
                Send(LD);
            }
        }
    }
}