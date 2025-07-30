using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTOs
{
	public class MessageEnvelope
	{
		public string Tipo { get; set; } = string.Empty; // "CreateOrder", "UpdateStatus", etc.
		public string Payload { get; set; } = string.Empty;
	}
}
