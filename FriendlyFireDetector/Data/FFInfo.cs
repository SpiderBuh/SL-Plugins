using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriendlyFireDetector.Data
{
	public class FFInfo
	{
		public FFInfo()
		{
			LastUpdate = DateTime.Now;
		}
		public string PlayerId { get; set; }
		public int Value { get; set; }
		public DateTime LastUpdate { get; set; }
	}
}
