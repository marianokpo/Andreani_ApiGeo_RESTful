using System;
using Geocodificador.Librery;

namespace Geocodificador
{
    class Program
    {
        private static bool FIN = false;

        public static void Main(string[] args)
        {
            Console.WriteLine("Start App Geocodificador");
            
            while(true)
            {
                RabbitMQData.Recive();
            }
            
        }

        
    }
}
