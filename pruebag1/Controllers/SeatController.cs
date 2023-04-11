using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using pruebag1.Models;
using System.Threading.Tasks;
using MongoDB.Driver;
using System.IO;
using pruebag1.Services;
using Microsoft.AspNetCore.Http;

namespace pruebag1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SeatController : ControllerBase
    {
        private readonly SeatService _seatService;

        public SeatController(SeatService seatService)
        {
            _seatService = seatService;           
        }
        
        //Obtener todos los asientos registrados en la coleccion "datos" de MongoDB
        [HttpGet]
        public ActionResult<Seat> GetSeats()
        {
            var todos = _seatService.Get();
            return Ok(todos);
        }

        //Obtener un asiento de la coleccion "datos" a través del identificador único "reference"
        [HttpGet("{id}")]
        public ActionResult<Seat> Get(string id)
        {
            var seat = _seatService.Get(id);

            if (seat == null)
            {
                return NotFound();
            }

            return seat;
        }

        //Carga en la coleccion "datos" un listado de asientos contables a través de un Json
        [HttpPost("json")]
        public ActionResult<Seat> CreateJson([FromBody] List<Seat> seats)
        {
            foreach(var item in seats)
            {
                Seat reg = new Seat
                {
                    Reference = item.Reference,
                    ReferenceDate = item.ReferenceDate,
                    Memo = item.Memo,
                    TaxDate = item.TaxDate,
                    DueDate = item.DueDate,
                    JournalEntryLines = item.JournalEntryLines,
                    StartDate = null,
                    EndDate = null,
                    LastUpdate = null,
                    NumberSap1 = 0,
                    NumberSap2 = 0,
                    ProcessId = null,
                    Status = "pendiente"
                };
                _seatService.Create(item);
            }
            return Ok();
        }

        //Carga en la coleccion "datos" un listado de asientos contables a través de un CSV
        [HttpPost("csv")]
        public ActionResult<Seat> CreateCSV(IFormFile ubicacionArchivo)
        {
            Stream s = ubicacionArchivo.OpenReadStream();
            StreamReader archivo = new StreamReader(s);
            string separador = ",";
            string linea;
            archivo.ReadLine();
            while ((linea = archivo.ReadLine()) != null)
            {
                string[] fila = linea.Split(separador);
                string referenceDate = fila[0];
                string memo = fila[1];
                string reference = fila[2];
                string taxDate = fila[3];
                string dueDate = fila[4];
                string lineId = fila[5];
                string accountCode = fila[6];
                string debit = fila[7];
                string credit = fila[8];
                string lineMemo = fila[9];
                string reference1 = fila[10];

                List<JournalEntryLines> list = new List<JournalEntryLines>();

                JournalEntryLines journal = new JournalEntryLines
                {
                    LineId = Convert.ToInt32(lineId),
                    AccountCode = accountCode,
                    Debit = Convert.ToDouble(debit),
                    Credit = Convert.ToDouble(credit),
                    LineMemo = lineMemo,
                    Reference1 = reference1,
                };
                list.Add(journal);

                Seat reg = new Seat
                {
                    Reference = reference,
                    ReferenceDate = DateTime.Parse(referenceDate),
                    Memo = memo,
                    TaxDate = DateTime.Parse(taxDate),
                    DueDate = DateTime.Parse(dueDate),
                    JournalEntryLines = list,
                    //Datos adicionales
                    StartDate = null,
                    EndDate = null,
                    LastUpdate = null,
                    NumberSap1 = 0,
                    NumberSap2 = 0,
                    ProcessId = null,
                    Status = "pendiente"
                };
                _seatService.Create(reg);
            }

            return Ok();
        }
    }
}
