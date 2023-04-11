using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace pruebag1.Models
{
    public class Seat
	{
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		public string _id { get; set; } //id que asigna Mongodb
		public string Reference { get; set; }
		public DateTime ReferenceDate { get; set; }
		public string Memo { get; set; }
		public DateTime TaxDate { get; set; }
		public DateTime DueDate { get; set; }
		public List<JournalEntryLines> JournalEntryLines { get; set; }

		//Datos adicionales
		public DateTime? StartDate { get; set; }
		public DateTime? EndDate { get; set; }
		public DateTime? LastUpdate { get; set; }
		public int NumberSap1 { get; set; }
		public int NumberSap2 { get; set; }
		public string ProcessId { get; set; }
		public string Status { get; set; }
	}
}
