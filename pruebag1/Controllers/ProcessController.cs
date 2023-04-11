using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using pruebag1.Models;
using System.Threading.Tasks;
using MongoDB.Driver;
using pruebag1.Services;

namespace pruebag1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProcessController : ControllerBase
    {
        private readonly SeatService _seatService;
        private readonly ProcessService _processService;

        public ProcessController(SeatService seatService, ProcessService processService)
        {
            _seatService = seatService;
            _processService = processService;
        }
        
        //Obtiene todos los procesos registrados
        [HttpGet]
        public ActionResult<Process> GetProcess()
        {
            var todos = _processService.Get();
            return Ok(todos);
        }

        //Obtiene un proceso registrado a través de el campo "Id" que es su identificador único
        [HttpGet("{id}")]
        public ActionResult<Process> Get(string id)
        {
            var proc = _processService.Get(id);

            if (proc == null)
            {
                return NotFound();
            }

            return proc;
        }

        //Crea un proceso con su identificador único
        [HttpPost]
        public ActionResult<Seat> Create([FromBody] List<Seat> seats)
        {
            //Identificador único del proceso
            var identifier = Guid.NewGuid().ToString();

            try
            {
                //Creo el proceso con status InProgress
                Process reg = new Process
                {
                    Id = identifier,
                    StartDate = DateTime.Now,
                    EndDate = null,
                    LastUpdate = null,
                    Items = 0,
                    Success = 0,
                    Failed = 0,
                    Status = "InProgress",
                };
                _processService.Create(reg);

                //Envío de asientos pendientes a SAP
                var procesar = _seatService.GetPendientes();

                //Contadores de items
                int items = procesar.Count();
                int success = 0;
                int failed = 0;

                foreach (var item in procesar)
                {
                    //Recibo respuesta, actualizo estado de asiento y contadores
                }

                //Actualizo estado y fecha de proceso
                return Ok(new { Success = true, ProcessId = identifier, Error = (string)null });
            }
            catch (Exception e)
            {
                return Ok(new { Success = true, ProcessId = identifier, Error = e.Message + " " + e.InnerException });
            }
        }

    }
}
