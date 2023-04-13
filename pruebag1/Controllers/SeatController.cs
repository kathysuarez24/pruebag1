using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using pruebag1.Models;
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
                _seatService.Create(reg);
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
                string lineId1 = fila[5];
                string accountCode1 = fila[6];
                string debit1 = fila[7];
                string credit1 = fila[8];
                string lineMemo1 = fila[9];
                string reference1 = fila[10];
                string lineId2 = fila[11];
                string accountCode2 = fila[12];
                string debit2 = fila[13];
                string credit2 = fila[14];
                string lineMemo2 = fila[15];
                string reference2 = fila[16];

                //Cada asiento debe tener dos entradas
                List<JournalEntryLines> list = new List<JournalEntryLines>();

                JournalEntryLines journal1 = new JournalEntryLines
                {
                    LineId = Convert.ToInt32(lineId1),
                    AccountCode = accountCode1,
                    Debit = Convert.ToDouble(debit1),
                    Credit = Convert.ToDouble(credit1),
                    LineMemo = lineMemo1,
                    Reference1 = reference1,
                };
                JournalEntryLines journal2 = new JournalEntryLines
                {
                    LineId = Convert.ToInt32(lineId2),
                    AccountCode = accountCode2,
                    Debit = Convert.ToDouble(debit2),
                    Credit = Convert.ToDouble(credit2),
                    LineMemo = lineMemo2,
                    Reference1 = reference2,
                };
                list.Add(journal1);
                list.Add(journal2);

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
