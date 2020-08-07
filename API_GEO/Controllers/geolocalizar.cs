using System;
using Microsoft.AspNetCore.Mvc;

using API_GEO.Librery;
using API_GEO.Models;
using System.Threading;

namespace API_GEO.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class geolocalizar : ControllerBase
    {
        private readonly ApiContext _context;

        public geolocalizar(ApiContext context)
        {
            _context = context;

            Thread th = new Thread( () => 
            {
                RabbitMQDataRetrive();
            });
            th.Start();
        }

        private void RabbitMQDataRetrive()
        {
            while (true)
            {
                RabbitMQData.Recive(_context);
            }
        }
        
        [HttpGet]
        public IActionResult Get([FromQuery]long id)
        {
            SQLClient SQLC = new SQLClient(_context);
            LocalizadorData LD = SQLC.GetID(id);
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

        [HttpPost]
        public IActionResult Post([FromBody]LocalizadorData Value)
        {
            
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