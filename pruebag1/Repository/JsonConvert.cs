using pruebag1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace pruebag1.Repository
{
    public class JsonConvert
    {
        /*public JsonConverter ()
        {
        }*/

        //Serializar Json en string
        public string SerializeToJson(Seat seat)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
            };
            Console.WriteLine(JsonSerializer.Serialize(seat, options));
            return JsonSerializer.Serialize(seat, options);
        }

        //Deserializar Json de string a objeto
        public Response DeserializeFromJson(string jsonString)
        {
            return JsonSerializer.Deserialize<Response>(jsonString);
        }

    }

}