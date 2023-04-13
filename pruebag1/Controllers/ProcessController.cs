using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using pruebag1.Models;
using System.Threading.Tasks;
using MongoDB.Driver;
using pruebag1.Services;
using pruebag1.Repository;
using System.Net.Http;
using System.Text.Json;
using System.Text;

namespace pruebag1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProcessController : ControllerBase
    {
        private readonly SeatService _seatService;
        private readonly ProcessService _processService;
        private readonly JsonConvert _repository;

        public ProcessController(SeatService seatService, ProcessService processService, JsonConvert repository)
        {
            _seatService = seatService;
            _processService = processService;
            _repository = repository;
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

        //Crea un proceso con su identificador único y envía asientos a SAP
        [HttpPost]
        public async Task<IActionResult> Create()
        {
            bool respuesta = false;
            int items = 0;
            int success = 0;
            int failed = 0;
            Response contentDes = new Response();
            Seat regSeatAct;

            //Identificador único del proceso
            var identifier = Guid.NewGuid().ToString();

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
            
            try
            {
                //Envío de asientos pendientes a SAP
                var procesar = _seatService.GetPendientes();
                items = procesar.Count();

                foreach (var item in procesar)
                {
                    var horaInicio = DateTime.Now;
                    var seatSerialized = _repository.SerializeToJson(item);

                    //Peticion a Api Externa
                    using (var httpClient = new HttpClient())
                    {
                        using var request = new HttpRequestMessage(new HttpMethod("POST"), "http://sakuragui.ddns.net:81/JournalEntries");
                        httpClient.DefaultRequestHeaders.Add("Company", "DEMO");
                        request.Content = new StringContent(seatSerialized, Encoding.UTF8, "application/json");
                        var response = await httpClient.SendAsync(request);
                        var content = await response.Content.ReadAsStringAsync();

                        //Si la petición es exitosa
                        if (response.IsSuccessStatusCode)
                        {
                            respuesta = true;
                            contentDes = _repository.DeserializeFromJson(content);
                        }
                        else
                        {
                            respuesta = false;
                        }
                    }

                    //Si el registro es exitoso
                    if (respuesta == true)
                    { 
                        int numberSap1 = contentDes.Data.DocEntry;
                        int numberSap2 = contentDes.Data.DocNum;

                        //Actualizo estado de asiento
                        regSeatAct = new Seat
                        {
                            _id = item._id,
                            Reference = item.Reference,
                            ReferenceDate = item.ReferenceDate,
                            Memo = item.Memo,
                            TaxDate = item.TaxDate,
                            DueDate = item.DueDate,
                            JournalEntryLines = item.JournalEntryLines,
                            StartDate = horaInicio,
                            EndDate = DateTime.Now,
                            LastUpdate = DateTime.Now,
                            NumberSap1 = numberSap1,
                            NumberSap2 = numberSap2,
                            ProcessId = identifier,
                            Status = "success"
                        };
                        _seatService.Update(item.Reference, regSeatAct);
                        success++;
                    }
                    else
                    {
                        //Actualizo estado de asiento
                        regSeatAct = new Seat
                        {
                            _id = item._id,
                            Reference = item.Reference,
                            ReferenceDate = item.ReferenceDate,
                            Memo = item.Memo,
                            TaxDate = item.TaxDate,
                            DueDate = item.DueDate,
                            JournalEntryLines = item.JournalEntryLines,
                            StartDate = horaInicio,
                            EndDate = null,
                            LastUpdate = DateTime.Now,
                            NumberSap1 = 0,
                            NumberSap2 = 0,
                            ProcessId = identifier,
                            Status = "failed"
                        };
                        _seatService.Update(item.Reference, regSeatAct);
                        failed++;
                    }
                }

                //Actualizo estado y fecha de proceso
                Process regAct = new Process
                {
                    _id = reg._id,
                    Id = identifier,
                    StartDate = reg.StartDate,
                    EndDate = DateTime.Now,
                    LastUpdate = DateTime.Now,
                    Items = items,
                    Success = success,
                    Failed = failed,
                    Status = "Success",
                };
                _processService.Update(identifier, regAct);

                //Respuesta solicitada solo para este metodo
                return Ok(new { Success= true, ProcessId= identifier, Error= (string)null});
            }
            catch (Exception e)
            {
                //Actualizo estado y fecha de proceso
                Process regAct = new Process
                {
                    _id = reg._id,
                    Id = identifier,
                    StartDate = reg.StartDate,
                    EndDate = DateTime.Now,
                    LastUpdate = DateTime.Now,
                    Items = items,
                    Success = success,
                    Failed = failed,
                    Status = "failed",
                };
                _processService.Update(identifier, regAct);
                return Ok(new { Success= false, ProcessId= identifier, Error= e.Message + " " + e.InnerException });
            }
        }

    }
}
