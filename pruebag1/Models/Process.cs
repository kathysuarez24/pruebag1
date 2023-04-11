using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace pruebag1.Models
{
	public class Process
	{
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		public string _id { get; set; }
		public string Id { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime? EndDate { get; set; }
		public DateTime? LastUpdate { get; set; }
		public int Items { get; set; }
		public int Success { get; set; }
		public int Failed { get; set; }
		public string Status { get; set; }
	}

}
