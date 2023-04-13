using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace pruebag1.Models
{
	//Estructura de la respuesta de Api Externa
	public class Response
    {
		public Data Data { get; set; }
		public string Message { get; set; }
	}

	public class Data
    {
		public int DocEntry { get; set; }
		public int DocNum { get; set; }
	}

}
