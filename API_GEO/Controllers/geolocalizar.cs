using System;
using Microsoft.AspNetCore.Mvc;

using API_GEO.Librery;
using API_GEO.Models;
using System.Threading;

namespace API_GEO.Controllers
{
    /// <summary>
    /// Clase de controller para el get y pos de la api.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class geolocalizar : ControllerBase
    {
        private readonly ApiContext _context;

        /// <summary>
        /// Constructor, se ejecuta tambien en un thread el sistema de escucha de RabbitMQ
        /// </summary>
        /// <param name="context"></param>
        public geolocalizar(ApiContext context)
        {
            _context = context;
            Thread th = new Thread( () => 
            {
                RabbitMQDataRetrive();
            });
            th.Start();
        }

        /// <summary>
        /// Ejecuta mientras el programa este en funcionameiento la escucha de RabbitMQ
        /// </summary>
         private void RabbitMQDataRetrive()
        {
            while (true)
            {
                string m = RabbitMQData.Recive(_context);
                if(m != "")
                {
                    /// <summary>
                    /// Procesado de la infrmacion recivida.
                    /// </summary>
                    ProcessInfo(m);
                }
            }
        }

        /// <summary>
        /// Se procesa el mensaje recivido para poder actualizar la base de datos.
        /// </summary>
        /// <param name="message"></param>
        private void ProcessInfo(string message)
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
                    case "lat":
                        LD.latitud = Convert.ToDouble(vr[1]);
                        break;
                    case "lon":
                        LD.longitud = Convert.ToDouble(vr[1]);
                        break;
                    default:
                        break;
                };
            }


            if(LD != null)
            {
                /// <summary>
                /// Actualiza la base de datos con los nuevos valores de latitud, longitud y estado.
                /// </summary>
                SQLClient.UpdateLanLon(LD.id,LD.latitud,LD.longitud,"TERMINADO");
            }
        }
        
        /// <summary>
        /// Get de la api la cual devuelve error id no encontrado si no se pasa informacion con formato json.
        /// si se envia la informacion correcta se obtiene la informacion del estado del progama.
        /// Estado Procesando: no se obtiene la informacion de la latitud y longitud y se indica que se esta procesando.
        /// Estado Terminado: se obtiene la latitud y la longitud
        /// </summary>
        /// <param name="value"></param>
        /// <returns>IActionResult</returns>
        [HttpGet]
        public IActionResult Get([FromBody] LocalizadorData value)
        {
            SQLClient SQLC = new SQLClient(_context);
            LocalizadorData LD = SQLC.GetID(value.id);
            if(LD != null && LD.id > 0)
            {
                return Ok(new string[] 
                {
                    "id:" + LD.id, 
                    "latitud:" + LD.latitud,
                    "longitud:" + LD.longitud,
                    "estado:" + LD.estado
                });
            }
            else
            {
                return BadRequest(new string[] {"id: Error"});
            }
            
        }

        /// <summary>
        /// Pos de la api donde se obtiene la nueva consulta a ser procesada y se devuelve el id correspondiente
        /// a la solicitud realizada.
        /// </summary>
        /// <param name="Value"></param>
        /// <returns>IActionResult</returns>
        [HttpPost]
        public IActionResult Post([FromBody] LocalizadorData Value)
        {
            //return Ok(Value.calle);
            if(Value.calle != "")
            {
                
                Value.estado = "PROCESANDO";
                SQLClient SQLC = new SQLClient(_context);
                long _id = SQLC.AddNew(Value);
                if(_id <= 0)
                {
                    return BadRequest("ERROR!");
                }
                else
                {
                    procesarDatos(Value);
                    return Accepted(new string[] {"id:" + _id });
                }
            }
            else
            {
                return BadRequest("ERROR!");
            }
        }


        /// <summary>
        /// esta funcion envia la informacion a RabbitMQ desde un thread.
        /// </summary>
        /// <param name="Value"></param>
        private void procesarDatos(LocalizadorData Value)
        {
            Thread th = new Thread( () => 
            {
                RabbitMQData.Send(Value);
            });
            th.Start();
        }
    }
}