using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriendlyFireDetector.Data
{
	public class FFCount : IComparable
	{
		public FFCount(int count)
		{
			Count = count;
		}

		public int Count { get; internal set; }
		public DateTime LastUpdate { get; internal set; }

		public void UpdateCount()
		{
			Count++;
			LastUpdate = DateTime.Now;
		}

		public int CompareTo(object other)
		{
			throw new NotImplementedException();
		}
	}
}
