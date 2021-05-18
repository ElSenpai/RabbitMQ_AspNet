using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Publisher.Models
{
	public class Message
	{
		public byte[] WordByte { get; set; }
        public string Email  { get; set; }
        public string FileName { get; set; }
    }
}
