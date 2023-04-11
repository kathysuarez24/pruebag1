using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace pruebag1.Models
{
	public class JournalEntryLines
    {
		public int LineId { get; set; }
		public string AccountCode { get; set; }
		public double Debit { get; set; }
		public double Credit { get; set; }
		public string LineMemo { get; set; }
		public string Reference1 { get; set; }
	}

}
